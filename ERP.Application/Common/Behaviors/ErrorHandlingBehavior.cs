using ERP.SharedKernel.Common;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Common.Behaviors;

/// <summary>
/// Catches exceptions thrown during command/query execution
/// and converts them to Result.Failure.
/// </summary>
public sealed class ErrorHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ErrorHandlingBehavior<TRequest, TResponse>> _logger;

    public ErrorHandlingBehavior(ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            _logger.LogInformation
                (
                "Domain rule violation in {RequestName}: {Message}",
                typeof(TRequest).Name, ex.Message
                );

            return CreateFailureResponse(ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation
                (
                "Resource not found in {RequestName}: {Message}",
                typeof(TRequest).Name, ex.Message
                );

            return CreateFailureResponse(ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation
                (
                "Validation failed in {RequestName}: {Errors}",
                typeof(TRequest).Name, ex.Message
                );

            return CreateFailureResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError
                (
                ex,
                "Unexpected error in {RequestName}: {Message}",
                typeof(TRequest).Name, ex.Message
                );

            return CreateFailureResponse("An unexpected error occurred. Please contact support.");
        }
    }

    private static TResponse CreateFailureResponse(string errorMessage)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errorMessage);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });
            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errorMessage })!;
            }
        }

        throw new InvalidOperationException
            (
            $"ErrorHandlingBehavior does not support response type {responseType.Name}."
            );
    }
}