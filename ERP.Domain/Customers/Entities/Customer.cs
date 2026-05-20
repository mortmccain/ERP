using ERP.Domain.Customers.Events;
using ERP.Domain.Customers.ValueObjects;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.Primitives;
using ERP.SharedKernel.ValueObjects;

namespace ERP.Domain.Customers.Entities;

/// <summary>
/// Represents a customer (company or individual) that purchases from the company.
/// This is a separate Aggregate from Sale — Sale references Customer by ID only.
/// </summary>
public sealed class Customer : AggregateRoot
{



    // ==================================================================================================================================
    //                                                          PROPERTIES
    // ==================================================================================================================================



    public CustomerCode CustomerCode { get; private set; }
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public string Phone { get; private set; }
    public Address BillingAddress { get; private set; }
    public Address ShippingAddress { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastOrderDate { get; private set; }



    // ==================================================================================================================================
    //                                                          CONSTRUCTORS
    // ==================================================================================================================================



    // disables the null reference warnings
#pragma warning disable CS8618
    private Customer() : base(Guid.Empty) { }
#pragma warning restore CS8618
    private Customer
        (
        CustomerCode customerCode,
        string name,
        string? email,
        string phone,
        Address billingAddress,
        Address shippingAddress
        ) : base(Guid.NewGuid())
    {
        if (customerCode is null) throw new DomainException("Customer code is required");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Customer name is required.");
        if (phone == null) throw new DomainException("Phone number is required");
        if (billingAddress is null) throw new DomainException("Billing address is required");
        if (shippingAddress is null) throw new DomainException("Shipping address is required");

        CustomerCode = customerCode;
        Name = name;
        Email = email;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }



    // ==================================================================================================================================
    //                                                          FACTORY METHODS
    // ==================================================================================================================================



    public static Customer Create
        (
        CustomerCode customerCode,
        string name,
        string? email,
        string phone,
        Address billingAddress,
        Address shippingAddress,
        bool isActive
        )
    {
        Customer customer = new Customer
            (
            customerCode,
            name,
            email,
            phone,
            billingAddress,
            shippingAddress
            );
        customer.IsActive = isActive;

        customer.AddDomainEvent
            (
            new CustomerCreatedDomainEvent      // might want to add created by user ID so we know who added this customer
            (
                customer.Id,
                customer.Name,
                customer.Phone,
                customer.BillingAddress,
                customer.ShippingAddress,
                customer.CreatedAt
            )
            );

        // Potential future concerns that belong here:
        // - Logging creation
        // - Publishing to service bus
        // - Assigning sequential CustomerCode

        return customer;

    }



    // ==================================================================================================================================
    //                                                          BEHAVIOR METHODS
    // ==================================================================================================================================



    public void UpdatePhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) throw new DomainException("Phone number can not be empty or null");

        Phone = phone;
    }

    // once set, it can not be cleared          should ? be deleted for this logic?
    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email value can not be replaced with epty or null values");

        Email = email;
    }

    public void UpdateBillingAddress(Address newAddress)
    {
        if (newAddress is null) throw new DomainException("Address can not be empty or null");

        BillingAddress = newAddress;
    }

    public void UpdateShippingAddress(Address newAddress)      
    {
        if (newAddress is null) throw new DomainException("Address can not be empty or null");

        ShippingAddress = newAddress;
    }



    /// <summary>
    /// Records that the customer placed an order.
    /// Called by a Domain Event handler when a Sale is created or approved.
    /// </summary>
    public void RecordOrder(DateTime orderDate)
    {
        if (orderDate > DateTime.UtcNow) throw new DomainException("Order date cannot be in the future.");

        LastOrderDate = orderDate;
    }

    /// <summary>
    /// Deactivates the customer. Prevents new sales but preserves historical data.
    /// </summary>
    public void Deactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("A reason is required for deactivation.");

        if (!IsActive) throw new DomainException("Customer is already inactive.");

        IsActive = false;

        AddDomainEvent(new CustomerDeactivatedDomainEvent(Id, CustomerCode, reason));
    }

    /// <summary>
    /// Reactivates a previously deactivated customer.
    /// </summary>
    public void Activate()
    {
        if (IsActive) throw new DomainException("Customer is already active.");

        IsActive = true;

        AddDomainEvent(new CustomerActivatedDomainEvent(Id, CustomerCode));
    }
}