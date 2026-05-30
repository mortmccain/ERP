using ERP.Application.Sales.DTOs;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Sales.Queries.GetSalesPaginated;

public sealed class GetSalesPaginatedQuery : IRequest<PaginatedResult<SaleListItemDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}