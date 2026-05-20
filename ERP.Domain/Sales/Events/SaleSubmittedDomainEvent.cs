using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;

namespace ERP.Domain.Sales.Events;

/// <summary>
/// Raised when a Sale transitions from Draft to Pending.
/// </summary>
public sealed class SaleSubmittedDomainEvent : BaseDomainEvent
{
    public Guid Id { get; }
    public Money Total { get; }

    public SaleSubmittedDomainEvent(Guid id, Money total)
    {
        Id = id;
        Total = total;
    }

}
