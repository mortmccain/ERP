using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;

namespace ERP.Application.Common.Interfaces;

public interface ISaleRepository
{
    // --- Queries ---
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Sale>> GetPaginatedBySpecificationAsync(
        Specification<Sale> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    // --- Commands ---
    void Add(Sale sale);
    void Remove(Sale sale);

    /*
     Why no Update method?

     EF Core tracks changes to entities loaded from the database. When a handler loads a Sale,
    modifies it, and calls UnitOfWork.SaveChangesAsync(), 
    EF Core automatically detects the changes and generates UPDATE statements.
    An explicit Update method is redundant.
     */
}