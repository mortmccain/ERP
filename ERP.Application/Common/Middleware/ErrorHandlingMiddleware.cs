using ERP.SharedKernel.Common;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace ERP.Application.Common.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<T> InvokeAsync<T>(
        Func<Task<T>> next,
        IMessageContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            _logger.LogInformation(
                "Domain rule violation in {RequestName}: {Message}",
                context.Message?.GetType().Name ?? "Unknown", ex.Message);

            return CreateFailureResponse<T>(ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation(
                "Resource not found in {RequestName}: {Message}",
                context.Message?.GetType().Name ?? "Unknown", ex.Message);

            return CreateFailureResponse<T>(ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation(
                "Validation failed in {RequestName}: {Errors}",
                context.Message?.GetType().Name ?? "Unknown", ex.Message);

            return CreateFailureResponse<T>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error in {RequestName}: {Message}",
                context.Message?.GetType().Name ?? "Unknown", ex.Message);

            return CreateFailureResponse<T>("An unexpected error occurred. Please contact support.");
        }
    }

    private static T CreateFailureResponse<T>(string errorMessage)
    {
        var responseType = typeof(T);

        if (responseType == typeof(Result))
        {
            return (T)(object)Result.Failure(errorMessage);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });
            if (failureMethod != null)
            {
                return (T)failureMethod.Invoke(null, new object[] { errorMessage })!;
            }
        }

        throw new InvalidOperationException(
            $"ErrorHandlingMiddleware does not support response type {responseType.Name}.");
    }
}