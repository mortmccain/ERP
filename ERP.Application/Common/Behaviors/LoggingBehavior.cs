using ERP.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Common.Behaviors;

/// <summary>
/// Logs the start and end of every command/query execution.
/// Includes the user who initiated the request and the outcome.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingBehavior
        (
        /*
         right here the logging behavior constructor is saying to the ILogger that it need a logger with it's own name (LoggingBehavior)
        on it. usually List<Car> means that the list has Car objects in it. in line of code,
        the ILogger takes the name of the logging behavioras the logger category name and slaps that name on the logger
        so we see the name in the logs. this apparantly is a miscrosoft issuewhich they prefered doing this instead of:

        ILogger.ForCategory("LoggingBehavior") 


        Why ILogger<LoggingBehavior<TRequest, TResponse>>?

        The generic type parameter on ILogger<T> creates a logger category.
        The category name becomes ERP.Application.Common.Behaviors.LoggingBehavior<CreateSaleCommand, Result<Guid>>.
        You can filter logs by category in production.

         */
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser
        )
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle
        (
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
        )
    {

        var requestName = typeof(TRequest).Name;        
        var userId = _currentUser.UserId;
        var username = _currentUser.Username;

        _logger.LogInformation
            (
            "Starting {RequestName} | User: {Username} ({UserId})",
            requestName, username, userId
            );

        var response = await next(cancellationToken);

        // Check if the response is a Result type to log success/failure
        if (response is ERP.SharedKernel.Common.Result result)
        {
            if (result.IsFailure)
            {
                _logger.LogWarning
                    (
                    "Failed {RequestName} | User: {Username} | Error: {Error}",
                    requestName, username, result.Error
                    );
            }
            else
            {
                _logger.LogInformation
                    (
                    "Completed {RequestName} | User: {Username} | Success",
                    requestName, username
                    );
            }
        }

        return response;
    }
}