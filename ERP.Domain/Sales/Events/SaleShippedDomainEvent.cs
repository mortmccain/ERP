
using ERP.SharedKernel.Common;

namespace ERP.Domain.Sales.Events;

/// <summary>
/// Raised when a Sale transitions from Approved to Shipped.
/// </summary>
public sealed class SaleShippedDomainEvent : BaseDomainEvent
{
    public Guid Id { get; }
    public DateTime ShippedAtUTC { get; }


    public SaleShippedDomainEvent(Guid id, DateTime shippedAt)
    {
        Id = id;
        ShippedAtUTC = shippedAt;
    }
}
