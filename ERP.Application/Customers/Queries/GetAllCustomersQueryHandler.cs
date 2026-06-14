using ERP.Application.Common.Interfaces;
using ERP.Application.Customers.DTOs;
using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;

namespace ERP.Application.Customers.Queries.GetAllCustomers;

public static class GetAllCustomersQueryHandler
{
    public static async Task<Result<List<CustomerListDto>>> Handle
        (
        GetAllCustomersQuery query,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken
        )
    {
        var customers = await customerRepository.GetBySpecificationAsync(
            Specification<Customer>.Empty,
            cancellationToken);

        var dtos = customers
            .Select(c => new CustomerListDto
            {
                Id = c.Id,
                CustomerCode = c.CustomerCode.Value,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive
            })
            .ToList();

        return Result<List<CustomerListDto>>.Success(dtos);
    }
}