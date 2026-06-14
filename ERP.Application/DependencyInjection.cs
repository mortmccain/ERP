using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.FluentValidation;

namespace ERP.Application;

/// <summary>
/// Centralizes registration of all Application layer services.
/// Call this from Program.cs in both WebUI and WebAPI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. FluentValidation (still needed)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. Wolverine configuration
        services.AddWolverine(opts =>
        {
            // Discover all message handlers (commands, queries, events) in this assembly
            opts.Discovery.IncludeAssembly(Assembly.GetExecutingAssembly());

            // Enable FluentValidation middleware (replaces ValidationBehavior)
            opts.AddFluentValidation();

            // Global middleware (executes in this order)
            opts.Policies.Add<LoggingMiddleware>();
            opts.Policies.Add<ErrorHandlingMiddleware>();
            opts.Policies.Add<PerformanceMiddleware>();

            // Configure local routing for domain events (stay in-process)
            opts.LocalRoutingConvention = true;
        });

        return services;
    }
}