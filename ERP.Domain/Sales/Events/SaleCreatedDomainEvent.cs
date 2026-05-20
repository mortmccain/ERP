
using ERP.Domain.Sales.ValueObjects;
using ERP.SharedKernel.Common;

namespace ERP.Domain.Sales.Events;

/// <summary>
/// Raised when a new Sale is created (in Draft status).
/// Handlers can use this to send notifications, initialize audit trails, etc.
/// </summary>
public sealed class SaleCreatedDomainEvent : BaseDomainEvent
{
    public Guid SaleId { get; }
    public SaleNumber SaleNumber { get; }
    public Guid CustomerId { get; }
    public string CustomerName { get; }
    public DateTime CreatedAtUtc { get; }
    public Guid CreatedByUserId { get; }
    public string CreatedByName { get; }

    public SaleCreatedDomainEvent
        (
        Guid saleId,
        SaleNumber saleNumber,
        Guid customerId,
        string customerName,
        DateTime createdAtUtc,
        Guid createByUserId,
        string createdByName
        )
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createByUserId;
        CreatedByName = createdByName;

    }
}