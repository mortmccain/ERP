using ERP.Application.Common.Interfaces;
using ERP.Application.Sales.DTOs;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.DTOs;

namespace ERP.Application.Sales.Queries.GetSaleById;

public static class GetSaleByIdQueryHandler
{
    public static async Task<Result<SaleDto>> Handle
        (
        GetSaleByIdQuery query,
        ISaleRepository saleRepository,
        CancellationToken cancellationToken
        )
    {
        var sale = await saleRepository.GetByIdAsync(query.SaleId, cancellationToken);

        if (sale is null)
            return Result<SaleDto>.Failure($"Sale '{query.SaleId}' was not found.");

        bool isAdminOrManager =
            query.UserRoles.Contains("Admin") || query.UserRoles.Contains("Manager");

        if (!isAdminOrManager && sale.CreatedByUserId != query.RequestedByUserId)
            return Result<SaleDto>.Failure("You do not have permission to view this sale.");

        var dto = new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber.Value,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            Status = sale.Status.ToString(),

            CreatedByUserId = sale.CreatedByUserId,
            CreatedByName = sale.CreatedByName,

            ShippingAddress = new AddressDto
            {
                Country = sale.ShippingAddress.Country,
                Street = sale.ShippingAddress.Street,
                City = sale.ShippingAddress.City,
                PostalCode = sale.ShippingAddress.PostalCode,
                ExactAddress = sale.ShippingAddress.ExactAddress

            },

            SubTotal = new MoneyDto { Amount = sale.SubTotal.Amount, Currency = sale.SubTotal.Currency },
            SubTotalAfterLineDiscounts = new MoneyDto { Amount = sale.SubTotalAfterLineDiscounts.Amount, Currency = sale.SubTotalAfterLineDiscounts.Currency },
            Discount = new MoneyDto { Amount = sale.Discount.Amount, Currency = sale.Discount.Currency },
            TaxableAmount = new MoneyDto { Amount = sale.TaxableAmount.Amount, Currency = sale.TaxableAmount.Currency },
            Tax = new MoneyDto { Amount = sale.Tax.Amount, Currency = sale.Tax.Currency },
            Total = new MoneyDto { Amount = sale.Total.Amount, Currency = sale.Total.Currency },

            DiscountPercentage = sale.DiscountPercentage,
            DiscountReason = sale.DiscountReason,
            TaxRate = sale.TaxRate,

            CreatedAtUtc = sale.CreatedAtUtc,
            ApprovedAtUtc = sale.ApprovedAtUtc,
            ApprovedByUserId = sale.ApprovedByUserId,
            ShippedAtUtc = sale.ShippedAtUtc,
            CancelledAtUtc = sale.CancelledAtUtc,
            CancellationReason = sale.CancellationReason,

            LineItems = sale.LineItems
                .OrderBy(li => li.LineNumber)
                .Select(li => new SaleLineItemDto
                {
                    Id = li.Id,
                    ProductId = li.ProductId,
                    ProductName = li.ProductName,
                    SKU = li.SKU,
                    Quantity = li.Quantity,
                    UnitPrice = new MoneyDto { Amount = li.UnitPrice.Amount, Currency = li.UnitPrice.Currency },
                    GrossTotal = new MoneyDto { Amount = li.GrossTotal.Amount, Currency = li.GrossTotal.Currency },
                    DiscountPercentage = li.DiscountPercentage,
                    DiscountReason = li.DiscountReason,
                    IsFreeOfCharge = li.IsFreeOfCharge,
                    FocReason = li.FocReason,
                    LineTotal = new MoneyDto { Amount = li.LineTotal.Amount, Currency = li.LineTotal.Currency },
                    LineNumber = li.LineNumber
                }).ToList(),

            SubmittedAtUtc = sale.SubmittedAtUtc,
            InvoicedAtUtc = sale.InvoicedAtUtc,
        };

        return Result<SaleDto>.Success(dto);
    }
}