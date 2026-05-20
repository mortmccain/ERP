
using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;

namespace ERP.Domain.Sales.Events;

public sealed class SaleLineItemUpdatedDomainEvent:BaseDomainEvent
{
    public Guid SaleId { get; }
    public Guid LineItemId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public int NewQuantity { get; }
    public Money UnitPrice { get; }
    public int LineNumber { get; }

    public SaleLineItemUpdatedDomainEvent
        (
        Guid saleId,
        Guid lineItemId,
        Guid productId,
        string productName,
        int newQuantity,
        Money unitPrice,
        int lineNumber
        )
    {
        SaleId = saleId;
        LineItemId = lineItemId;
        ProductId = productId;
        ProductName = productName;
        NewQuantity = newQuantity;
        UnitPrice = unitPrice;
        LineNumber = lineNumber;
    }
}
