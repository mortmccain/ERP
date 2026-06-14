using ERP.Application.Common.Interfaces;
using ERP.Application.Customers.DTOs;
using ERP.Application.Customers.Specifications;
using ERP.SharedKernel.Common;

namespace ERP.Application.Customers.Queries.GetActiveCustomersForSaleCreation;

public sealed class GetActiveCustomersForSaleCreationQueryHandler
    : IRequestHandler<GetActiveCustomersForSaleCreationQuery, Result<List<CustomerForSaleCreationDto>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetActiveCustomersForSaleCreationQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<List<CustomerForSaleCreationDto>>> Handle(
        GetActiveCustomersForSaleCreationQuery query,
        CancellationToken cancellationToken)
    {
        var specification = new ActiveCustomerSpecification();

        var customers = await _customerRepository.GetBySpecificationAsync(specification, cancellationToken);

        var dtos = customers
            .Select(c => new CustomerForSaleCreationDto
            {
                Id = c.Id,
                Name = c.Name,
                CustomerCode = c.CustomerCode.Value,
                Phone = c.Phone,
                Email = c.Email,
                DisplayName = $"{c.Name} ({c.CustomerCode.Value})"
            })
            .OrderBy(d => d.DisplayName)
            .ToList();

        return Result<List<CustomerForSaleCreationDto>>.Success(dtos);
    }
}