using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete implementation of ICustomerRepository using Entity Framework Core.
/// </summary>
public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public void Add(Customer customer)
    {
        _dbContext.Customers.Add(customer);
    }
}