using ERP.Application.Common.Interfaces;
using ERP.Application.Customers.DTOs;
using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Customers.Queries.GetAllCustomers;

public sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<List<CustomerListDto>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<List<CustomerListDto>>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetBySpecificationAsync(
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