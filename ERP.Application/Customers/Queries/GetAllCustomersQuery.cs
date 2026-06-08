using ERP.Application.Customers.DTOs;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Customers.Queries.GetAllCustomers;

public sealed class GetAllCustomersQuery : IRequest<Result<List<CustomerListDto>>>
{
}