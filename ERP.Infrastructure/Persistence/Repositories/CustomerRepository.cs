using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete implementation of ICustomerRepository using Entity Framework Core.
/// </summary>
public sealed class CustomerRepository : ICustomerRepository
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public CustomerRepository(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var context = await _factory.CreateDbContextAsync();

        return await context.Customers
        .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Customer>> GetBySpecificationAsync(
    Specification<Customer> specification,
    CancellationToken cancellationToken = default)
    {
        await using var context = await _factory.CreateDbContextAsync();

        return await context.Customers
            .Where(specification.ToExpression())
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var context = await _factory.CreateDbContextAsync();

        return await context.Customers
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await using var context = await _factory.CreateDbContextAsync(ct);
        context.Customers.Add(customer);
        await context.SaveChangesAsync(ct);
    }
}