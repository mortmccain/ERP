using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete implementation of ISaleRepository using Entity Framework Core.
/// </summary>
public sealed class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.LineItems)      // eager loading since an aggregate root is incomplete without it's child entities
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<PaginatedResult<Sale>> GetPaginatedBySpecificationAsync
        (
        Specification<Sale> specification,  // this is polymorphism. specification is an abstract class and can't have an
                                            // object. however, any class that implements it can be passed here.
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
        )
    {
        // Count total matching records
        var totalCount = await _context.Sales
            .Where(specification.ToExpression())
            .CountAsync(cancellationToken);

        // Fetch the requested page
        var items = await _context.Sales
            .Include(s => s.LineItems)
            .Where(specification.ToExpression())
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)      // if the user wants the 5th page, skip the first 4 pages
            .Take(pageSize)                        // take the amount of items present in one page
            .ToListAsync(cancellationToken);      // actually gets the items and lists them

        return new PaginatedResult<Sale>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<List<Sale>> GetAllBySpecificationAsync
        (
        Specification<Sale> specification,
        CancellationToken cancellationToken = default
        )
    {
        // No .Include(s => s.LineItems) — dashboard only reads Sale-level aggregate fields.
        // Total, Status, CreatedAtUtc and CreatedByUserId are all on the Sale root itself.

        return await _context.Sales
            .Where(specification.ToExpression())
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    // Write – now uses shared context; SaveChanges handled by UnitOfWork
    public async Task AddAsync(Sale sale, CancellationToken ct)
    {
        _context.Sales.Add(sale);
        // SaveChanges removed — handled by UnitOfWork in the same shared context/transaction
    }

    public async Task RemoveAsync(Sale sale, CancellationToken ct)
    {
        _context.Sales.Remove(sale);
        // SaveChanges removed — handled by UnitOfWork in the same shared context/transaction
    }
}