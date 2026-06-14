using ERP.Application.Common.Interfaces;
using ERP.Application.Customers.DTOs;
using ERP.Application.Customers.Specifications;
using ERP.SharedKernel.Common;

namespace ERP.Application.Customers.Queries.GetActiveCustomersForSaleCreation;

public static class GetActiveCustomersForSaleCreationQueryHandler
{
    public static async Task<Result<List<CustomerForSaleCreationDto>>> Handle
        (
        GetActiveCustomersForSaleCreationQuery query,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken
        )
    {
        var specification = new ActiveCustomerSpecification();

        var customers = await customerRepository.GetBySpecificationAsync(specification, cancellationToken);

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