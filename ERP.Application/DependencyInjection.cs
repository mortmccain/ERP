using System.Reflection;
using ERP.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Application;

/// <summary>
/// Centralizes registration of all Application layer services.
/// Call this from Program.cs in both WebUI and WebAPI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR — discovers all Command/Query handlers in this assembly
        services.AddMediatR
        (
                cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            }
        );

        // Register FluentValidation — discovers all Validators in this assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register pipeline behaviors (order matters — outermost first)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }
}