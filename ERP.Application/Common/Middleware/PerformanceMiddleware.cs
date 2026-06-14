using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace ERP.Application.Common.Middleware;

public sealed class PerformanceMiddleware
{
    private readonly ILogger<PerformanceMiddleware> _logger;
    private const int WarningThresholdMs = 500;

    public PerformanceMiddleware(ILogger<PerformanceMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<T> InvokeAsync<T>(
        Func<Task<T>> next,
        IMessageContext context,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > WarningThresholdMs)
        {
            _logger.LogWarning(
                "Slow request: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                context.Message?.GetType().Name ?? "Unknown",
                stopwatch.ElapsedMilliseconds,
                WarningThresholdMs);
        }

        return response;
    }
}