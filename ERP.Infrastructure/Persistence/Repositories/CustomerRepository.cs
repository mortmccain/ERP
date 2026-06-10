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
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Customer>> GetBySpecificationAsync(
        Specification<Customer> specification,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(specification.ToExpression())
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        _context.Customers.Add(customer);
        // SaveChanges removed — handled by UnitOfWork in the same shared context/transaction
    }
}