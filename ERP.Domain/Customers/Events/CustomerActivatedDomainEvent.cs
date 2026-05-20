
using ERP.Domain.Customers.ValueObjects;
using ERP.SharedKernel.Common;

namespace ERP.Domain.Customers.Events;

public sealed class CustomerActivatedDomainEvent : BaseDomainEvent
{
    public Guid Id { get; }
    public CustomerCode CustomerCode { get; }

    public CustomerActivatedDomainEvent(Guid id, CustomerCode customerCode)
    {
        Id = id;
        CustomerCode = customerCode;
    }
}
