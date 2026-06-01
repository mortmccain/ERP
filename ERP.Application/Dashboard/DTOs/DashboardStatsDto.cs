namespace ERP.Application.Dashboard.DTOs;

public sealed class DashboardStatsDto
{
    // ── Summary card data ────────────────────────────────────────────────
    public int TotalSalesCount { get; init; }
    public int DraftSalesCount { get; init; }
    public int PendingSalesCount { get; init; }
    public int ApprovedSalesCount { get; init; }
    public int CancelledSalesCount { get; init; }
    public int ActiveCustomersCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public string Currency { get; init; } = "IRI";

    // ── Chart data ───────────────────────────────────────────────────────
    /// <summary>One entry per year for the last 5 calendar years.</summary>
    public List<YearlyDataPoint> YearlyData { get; init; } = new();

    /// <summary>One entry per month (Jan–Dec) for the current year.</summary>
    public List<MonthlyDataPoint> CurrentYearMonthlyData { get; init; } = new();

    /// <summary>One entry per status that has at least one sale.</summary>
    public List<StatusDataPoint> StatusBreakdown { get; init; } = new();

    // Nested records used as Radzen chart data points.
    // Property names match the CategoryProperty / ValueProperty strings on each series.
    public sealed record YearlyDataPoint(string YearLabel, decimal TotalAmount, int Count);
    public sealed record MonthlyDataPoint(string MonthLabel, decimal TotalAmount, int Count);
    public sealed record StatusDataPoint(string Status, int Count);
}