using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;

namespace ERP.Domain.Sales.Events;

/// <summary>
/// Raised when a line item is added to an existing Sale.
/// </summary>
public sealed class SaleLineItemAddedDomainEvent : BaseDomainEvent
{
    public Guid SaleId { get; }
    public Guid LineItemId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public Money UnitPrice { get; }
    public int LineNumber { get; }

    public SaleLineItemAddedDomainEvent(
        Guid saleId,
        Guid lineItemId,
        Guid productId,
        string productName,
        int quantity,
        Money unitPrice,
        int lineNumber)
    {
        SaleId = saleId;
        LineItemId = lineItemId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineNumber = lineNumber;
    }
}
