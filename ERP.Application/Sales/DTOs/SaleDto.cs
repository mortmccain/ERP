using ERP.SharedKernel.DTOs;

namespace ERP.Application.Sales.DTOs;

public sealed class SaleDto
{
    public Guid Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;

    // --- Creator ---
    public Guid CreatedByUserId { get; init; }
    public string CreatedByName { get; init; } = string.Empty;

    // --- Shipping ---
    public AddressDto ShippingAddress { get; init; } = null!;

    // --- Financial ---
    public MoneyDto SubTotal { get; init; } = null!;
    public MoneyDto SubTotalAfterLineDiscounts { get; init; } = null!;
    public MoneyDto Discount { get; init; } = null!;
    public MoneyDto TaxableAmount { get; init; } = null!;
    public MoneyDto Tax { get; init; } = null!;
    public MoneyDto Total { get; init; } = null!;
    public decimal? DiscountPercentage { get; init; }
    public string? DiscountReason { get; init; }
    public decimal? TaxRate { get; init; }

    // --- Timestamps ---
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? SubmittedAtUtc { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public Guid? ApprovedByUserId { get; init; }
    public DateTime? ShippedAtUtc { get; init; }
    public DateTime? InvoicedAtUtc { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? CancellationReason { get; init; }

    // --- Line Items ---
    public List<SaleLineItemDto> LineItems { get; init; } = new();
}