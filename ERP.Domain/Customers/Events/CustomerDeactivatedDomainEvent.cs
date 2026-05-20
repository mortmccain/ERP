using ERP.Domain.Customers.ValueObjects;
using ERP.SharedKernel.Common;

namespace ERP.Domain.Customers.Events;

public sealed class CustomerDeactivatedDomainEvent : BaseDomainEvent
{

    public Guid Id { get; }
    public CustomerCode CustomerCode { get; }
    public string Reason { get; }

    public CustomerDeactivatedDomainEvent(Guid id, CustomerCode customerCode, string reason)
    {
        Id = id;
        CustomerCode = customerCode;
        Reason = reason;
    }
}
