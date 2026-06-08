using ERP.Application.Customers.DTOs;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Customers.Queries.GetActiveCustomersForSaleCreation;

public sealed class GetActiveCustomersForSaleCreationQuery : IRequest<Result<List<CustomerForSaleCreationDto>>>
{
    // No parameters needed – the page is already gated by [Authorize].
    // If you ever need to limit customers by user or role you can add
    // RequestedByUserId/UserRoles later without breaking the contract.
}