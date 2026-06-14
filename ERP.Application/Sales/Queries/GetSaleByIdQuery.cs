using ERP.Application.Sales.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Sales.Queries.GetSaleById;

public sealed class GetSaleByIdQuery : IRequest<Result<SaleDto>>
{
    public Guid SaleId { get; init; }
    public Guid RequestedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}