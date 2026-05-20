
using ERP.SharedKernel.DTOs;

namespace ERP.Application.Sales.DTOs;

// DTO for output
public sealed class SaleLineItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? SKU { get; init; }
    public int Quantity { get; init; }
    public MoneyDto UnitPrice { get; init; } = null!;
    public MoneyDto GrossTotal { get; init; } = null!;
    public decimal DiscountPercentage { get; init; }
    public string? DiscountReason { get; init; }
    public bool IsFreeOfCharge { get; init; }
    public string? FocReason { get; init; }
    public MoneyDto LineTotal { get; init; } = null!;
    public int LineNumber { get; init; }
}
