
using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;


namespace ERP.Domain.Customers.Events;

public sealed class CustomerCreatedDomainEvent : BaseDomainEvent
{
    public Guid CustomerId { get; }
    public string Name { get; }
    public string Phone { get; }
    public Address BillingAddress { get; }
    public Address ShippingAddress { get; }
    public DateTime CreatedAt { get; }


    public CustomerCreatedDomainEvent        // might want to add created by user ID so we know who added this customer
        (
        Guid customerId,
        string name,
        string phone,
        Address billingAddress,
        Address shippingAddress,
        DateTime createdAt
        )

    {
        CustomerId = customerId;
        Name = name;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        CreatedAt = createdAt;
    }

}