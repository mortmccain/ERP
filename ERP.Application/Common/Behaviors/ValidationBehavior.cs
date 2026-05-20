using ERP.SharedKernel.Common;
using FluentValidation;
using MediatR;

namespace ERP.Application.Common.Behaviors;

/// <summary>
/// Validates the request before it reaches the handler.
/// If validation fails, returns a failure Result without executing the handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    // this line gets all the validators and injects them here (validations must inherit from AbstractValidator<>)
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        // does this line get all the validators in the program or the property line above? 
        _validators = validators;
    }

    public async Task<TResponse> Handle
        (
        TRequest request,
        RequestHandlerDelegate<TResponse> next, // why does request handler delegate need the TResponse?
        CancellationToken cancellationToken
        )
    {
        if (!_validators.Any())
        {
            // No validators registered for this request type. Continue.
            return await next(cancellationToken);
        }
        // context is the request (command / query) wrapped plus some optional metadata like which properties to validate
        // or a dictionary of custom state that we want available during validation
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll
            (
            // runs all the validations gathered inside _validators with extra context and ct
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

        var failures = validationResults
            .SelectMany(r => r.Errors)      // the Errors property inside ValidationResult class is a list of validationFailure
            .Where(f => f != null)         // so every Error is a ValidationFailure. F is for Failure
            .ToList();

        if (failures.Any())
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            return CreateFailureResponse(errorMessage);
        }

        return await next(cancellationToken);
    }



    /// <summary>
    /// Creates a failure response of the correct Result type.
    /// Uses reflection because TResponse could be Result or Result<T>.
    /// </summary>
    private static TResponse CreateFailureResponse(string errorMessage)
    {
        var responseType = typeof(TResponse);
        // If TResponse is Result (non-generic)
        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errorMessage);
        }

        // If TResponse is Result<T> (generic), we need to call Result<T>.Failure()
        // we check the "responseType.IsGenericType" because calling "GetGenericTypeDefinition()" on anything like string or int or
        // whatever that's not generic, casues an exception being thrown
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            // calls the failure method of Result<> that has a parameter of type string 
            var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });
            if (failureMethod != null)
            {
                // casts the object of type object created by the invoke into a TResponse because c# is retarded
                // invoke tells the function that it doesn't run on any objects (since the failure method is static) and it does it by
                // the null property. the array is the parameters that the function might get and that string is the parameter that 
                // result gets
                return (TResponse)failureMethod.Invoke(null, new object[] { errorMessage })!;
            }
        }

        throw new InvalidOperationException
            (
            $"ValidationBehavior does not support response type {responseType.Name}. " + "Response must be Result or Result<T>."
            );
    }
}