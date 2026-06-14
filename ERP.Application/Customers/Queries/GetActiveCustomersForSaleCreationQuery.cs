using ERP.Application.Customers.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Customers.Queries.GetActiveCustomersForSaleCreation;

public sealed class GetActiveCustomersForSaleCreationQuery
{
    // No parameters needed – the page is already gated by [Authorize].
    // If you ever need to limit customers by user or role you can add
    // RequestedByUserId/UserRoles later without breaking the contract.
}