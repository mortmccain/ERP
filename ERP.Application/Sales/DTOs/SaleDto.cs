using ERP.SharedKernel.DTOs;

namespace ERP.Application.Sales.DTOs;

public sealed class SaleDto
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public MoneyDto SubTotal { get; init; } = null!;
    public MoneyDto Discount { get; init; } = null!;
    public MoneyDto TaxableAmount { get; init; } = null!;
    public MoneyDto Tax { get; init; } = null!;
    public MoneyDto Total { get; init; } = null!;
    public decimal? DiscountPercentage { get; init; }
    public string? DiscountReason { get; init; }
    public decimal? TaxRate { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public List<SaleLineItemDto> LineItems { get; init; } = new();
}
