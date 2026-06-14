using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Customers.Commands.CreateCustomer;

public static class CreateCustomerCommandHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateCustomerCommand command,
        ICustomerCodeGenerator customerCodeGenerator,
        IUnitOfWork unitOfWork,
        ILogger<CreateCustomerCommand> logger,
        CancellationToken cancellationToken)
    {
        // STEP 1: Generate a CustomerCode
        var customerCode = await customerCodeGenerator.NextAsync("CUST", cancellationToken);

        // STEP 2: Build Address value objects from the flat command inputs
        var billingAddress = new Address(
            command.BillingAddress.Country,
            command.BillingAddress.Street,
            command.BillingAddress.City,
            command.BillingAddress.PostalCode,
            command.BillingAddress.ExactAddress);

        var shippingAddress = new Address(
            command.ShippingAddress.Country,
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.PostalCode,
            command.ShippingAddress.ExactAddress);

        // STEP 3: Create the Customer Aggregate
        var customer = Customer.Create(
            customerCode,
            command.Name,
            command.Email,
            command.Phone,
            billingAddress,
            shippingAddress,
            command.IsActive);

        // STEP 4: Persist
        await unitOfWork.AddAsync<Customer>(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // STEP 5: Log and return
        logger.LogInformation(
            "Created Customer {CustomerCode} - {CustomerName}. CustomerId: {CustomerId}",
            customer.CustomerCode,
            customer.Name,
            customer.Id);

        return Result<Guid>.Success(customer.Id);
    }
}