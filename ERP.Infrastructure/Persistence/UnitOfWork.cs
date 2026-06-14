using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace ERP.Infrastructure.Persistence;

/// <summary>
/// Concrete implementation of IUnitOfWork.
/// Coordinates persistence across all repositories and dispatches domain events.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IMessageBus _messageBus;

    public UnitOfWork(AppDbContext context, IMessageBus messageBus)
    {
        _context = context;
        _messageBus = messageBus;
    }

    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
    {
        return await _context.FindAsync<T>(id, cancellationToken);
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        await _context.AddAsync(entity, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // STEP 1: Dispatch all domain events collected by tracked Aggregate Roots.
        // This happens BEFORE SaveChanges so that event handlers can add
        // additional changes to the same transaction.
        await DispatchDomainEventsAsync();

        // STEP 2: Persist all tracked changes to the database in a single transaction.
        int result = await _context.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<T?> GetByIdAsync<T>(Guid id,
        IEnumerable<string>? includes = null,
        CancellationToken cancellationToken = default) where T : class
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes is not null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    /// <summary>
    /// Finds all tracked Aggregate Roots that have pending domain events,
    /// publishes those events via MediatR, and clears them.
    /// </summary>
    private async Task DispatchDomainEventsAsync()
    {
        // Get all tracked Aggregate Roots that have domain events
        var aggregateRoots = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)    // if any domain events exist,...
            .Select(entry => entry.Entity)
            .ToList();  // if this wasn't here, the aggregateRoots variable wouldn't get materialized, so domain events would be
                        // empty since the LINQ query (lazy) would execute after events are cleared.

        // Collect all domain events
        var domainEvents = aggregateRoots
            .SelectMany(root => root.DomainEvents)
            .ToList();

        // Clear events immediately so they aren't re-dispatched
        foreach (var root in aggregateRoots)
        {
            /*
             If a handler throws an exception,
             we don't want the events to remain on the Aggregate.
             On the next SaveChanges, they'd be re-published
             */
            root.ClearDomainEvents();
        }

        // Publish each event via MediatR
        // Handlers run in the same process and can add more changes
        // to the DbContext before SaveChanges commits everything.
        foreach (var domainEvent in domainEvents)
        {
            await _messageBus.PublishAsync(domainEvent);
        }
    }

    public void Dispose()
    {
        // Context lifetime managed by DI (scoped); no manual dispose needed here in most cases
    }
}