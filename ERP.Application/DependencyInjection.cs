using ERP.Application.Common.Middleware;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Wolverine;
using Wolverine.FluentValidation;

namespace ERP.Application;

/// <summary>
/// Centralizes registration of all Application layer services.
/// Call from Program.cs in both WebUI and WebAPI.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures Wolverine message bus options (handler discovery, middleware, FluentValidation).
    /// Must be called via builder.Host.UseWolverine(...) in the host project.
    /// </summary>
    public static void ConfigureWolverine(WolverineOptions options)
    {
        // Discover all command/query/event handlers in this assembly
        options.Discovery.IncludeAssembly(Assembly.GetExecutingAssembly());

        // Global middleware — order matters (outermost first, innermost last)
        // Mirrors original MediatR pipeline: Logging → Performance
        options.Policies.AddMiddleware < LoggingMiddleware > ();
        options.Policies.AddMiddleware < PerformanceMiddleware > ();

        // FluentValidation (replaces ValidationBehavior; runs before handlers)
        // Throws ValidationException on failure, which the Blazor try/catch handles.
        options.UseFluentValidation();

        // NOTE: ServiceLocationPolicy / AlwaysUseServiceLocationFor is NOT set here
        // because AppDbContext lives in Infrastructure. The host project (WebUI/WebAPI)
        // adds that after calling this method.
    }

    /// <summary>
    /// Registers Application layer services into DI.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators into DI container
        // (needed by Wolverine.FluentValidation middleware)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}