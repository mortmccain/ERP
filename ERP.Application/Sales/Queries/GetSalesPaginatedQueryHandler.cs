using ERP.Application.Common.Interfaces;
using ERP.Application.Sales.DTOs;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.DTOs;
using MediatR;

namespace ERP.Application.Sales.Queries.GetSalesPaginated;

public sealed class GetSalesPaginatedQueryHandler
    : IRequestHandler<GetSalesPaginatedQuery, PaginatedResult<SaleListItemDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesPaginatedQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<PaginatedResult<SaleListItemDto>> Handle(
        GetSalesPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var spec =  Specification<Sale>.Empty;

        var paginated = await _saleRepository.GetPaginatedBySpecificationAsync(
            spec, query.PageNumber, query.PageSize, cancellationToken);

        var dtos = paginated.Items.Select
            (
            s => new SaleListItemDto
        {
            Id = s.Id,
            SaleNumber = s.SaleNumber.Value,
            CustomerName = s.CustomerName,
            Status = s.Status.ToString(),
            Total = new MoneyDto
            {
                Amount = s.Total.Amount,
                Currency = s.Total.Currency
            },
            CreatedAtUtc = s.CreatedAtUtc
        }
            ).ToList();

        return new PaginatedResult<SaleListItemDto>(
            dtos, paginated.TotalCount, query.PageNumber, query.PageSize);
    }
}