using ERP.Application.Common.Interfaces;
using ERP.Application.Dashboard.DTOs;
using ERP.Application.Sales.Specifications;
using ERP.Domain.Customers.Entities;
using ERP.Domain.Sales.Entities;
using ERP.Domain.Sales.Enums;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Dashboard.Queries.GetDashboardStats;

public sealed class GetDashboardStatsQueryHandler
    : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetDashboardStatsQueryHandler(
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<DashboardStatsDto>> Handle(
        GetDashboardStatsQuery query,
        CancellationToken cancellationToken)
    {
        // ReadOnly users see company-wide data just like Admin and Manager.
        // Only plain Employee role is scoped to their own sales.
        bool isEmployee = !query.UserRoles.Contains("Admin")
                       && !query.UserRoles.Contains("Manager")
                       && !query.UserRoles.Contains("ReadOnly");

        Specification<Sale> spec = isEmployee
            ? new SaleByCreatorSpecification(query.RequestedByUserId)
            : Specification<Sale>.Empty;

        // Load matching sales — no line items needed for dashboard aggregations
        var allSales = await _saleRepository.GetAllBySpecificationAsync(spec, cancellationToken);

        // Customers are always company-wide; employees still need to know who to sell to
        var allCustomers = await _customerRepository.GetBySpecificationAsync(
            Specification<Customer>.Empty, cancellationToken);

        int currentYear = DateTime.UtcNow.Year;
        int firstYear = currentYear - 4;           // 5 years inclusive

        var dto = new DashboardStatsDto
        {
            // ── Summary counts ───────────────────────────────────────────
            TotalSalesCount = allSales.Count,
            DraftSalesCount = allSales.Count(s => s.Status == SaleStatus.Draft),
            PendingSalesCount = allSales.Count(s => s.Status == SaleStatus.Pending),
            ApprovedSalesCount = allSales.Count(s => s.Status == SaleStatus.Approved),
            CancelledSalesCount = allSales.Count(s => s.Status == SaleStatus.Cancelled),
            ActiveCustomersCount = allCustomers.Count(c => c.IsActive),
            TotalRevenue = allSales
                .Where(s => s.Status != SaleStatus.Cancelled)
                .Sum(s => s.Total.Amount),
            Currency = "IRI",

            // ── 5-year data (pie + column charts) ────────────────────────
            YearlyData = Enumerable.Range(firstYear, 5)
                .Select(year =>
                {
                    var ys = allSales.Where(s => s.CreatedAtUtc.Year == year).ToList();
                    return new DashboardStatsDto.YearlyDataPoint(
                        YearLabel: year.ToString(),
                        TotalAmount: ys.Where(s => s.Status != SaleStatus.Cancelled)
                                       .Sum(s => s.Total.Amount),
                        Count: ys.Count);
                })
                .ToList(),

            // ── Monthly trend for current year (line chart) ───────────────
            CurrentYearMonthlyData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var ms = allSales
                        .Where(s => s.CreatedAtUtc.Year == currentYear
                                 && s.CreatedAtUtc.Month == month)
                        .ToList();
                    return new DashboardStatsDto.MonthlyDataPoint(
                        MonthLabel: new DateTime(currentYear, month, 1).ToString("MMM"),
                        TotalAmount: ms.Where(s => s.Status != SaleStatus.Cancelled)
                                       .Sum(s => s.Total.Amount),
                        Count: ms.Count);
                })
                .ToList(),

            // ── Status breakdown (donut chart) ────────────────────────────
            // Only include statuses that actually have sales so the chart isn't cluttered
            StatusBreakdown = Enum.GetValues<SaleStatus>()
                .Select(status => new DashboardStatsDto.StatusDataPoint(
                    Status: status.ToString(),
                    Count: allSales.Count(s => s.Status == status)))
                .Where(p => p.Count > 0)
                .ToList()
        };

        return Result<DashboardStatsDto>.Success(dto);
    }
}