using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerCodeGenerator _customerCodeGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ICustomerCodeGenerator customerCodeGenerator,
        IUnitOfWork unitOfWork,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _customerCodeGenerator = customerCodeGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        // STEP 1: Generate a CustomerCode
        var customerCode = await _customerCodeGenerator.NextAsync("CUST", cancellationToken);

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
        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // STEP 5: Log and return
        _logger.LogInformation(
            "Created Customer {CustomerCode} - {CustomerName}. CustomerId: {CustomerId}",
            customer.CustomerCode,
            customer.Name,
            customer.Id);

        return Result<Guid>.Success(customer.Id);
    }
}