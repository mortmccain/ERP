using ERP.Application.Dashboard.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Dashboard.Queries.GetDashboardStats;

public sealed class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
{
    public Guid RequestedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}