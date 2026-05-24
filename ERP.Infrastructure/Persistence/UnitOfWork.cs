using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Primitives;
using MediatR;

namespace ERP.Infrastructure.Persistence;

/// <summary>
/// Concrete implementation of IUnitOfWork.
/// Coordinates persistence across all repositories and dispatches domain events.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IMediator _mediator;

    public UnitOfWork(AppDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // STEP 1: Dispatch all domain events collected by tracked Aggregate Roots.
        // This happens BEFORE SaveChanges so that event handlers can add
        // additional changes to the same transaction.
        await DispatchDomainEventsAsync();

        // STEP 2: Persist all tracked changes to the database in a single transaction.
        int result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    /// <summary>
    /// Finds all tracked Aggregate Roots that have pending domain events,
    /// publishes those events via MediatR, and clears them.
    /// </summary>
    private async Task DispatchDomainEventsAsync()
    {
        // Get all tracked Aggregate Roots that have domain events
        var aggregateRoots = _dbContext.ChangeTracker
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
            await _mediator.Publish(domainEvent);
        }
    }
}