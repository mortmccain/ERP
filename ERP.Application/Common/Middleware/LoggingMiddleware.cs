using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Common;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace ERP.Application.Common.Middleware;

public sealed class LoggingMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<T> InvokeAsync<T>(
        Func<Task<T>> next,
        IMessageContext context,
        CancellationToken cancellationToken)
    {
        var requestName = context.Message?.GetType().Name ?? "Unknown";

        _logger.LogInformation(
            "Starting {RequestName} | User: {Username} ({UserId})",
            requestName, _currentUser.Username, _currentUser.UserId);

        var response = await next();

        if (response is Result result)
        {
            if (result.IsFailure)
            {
                _logger.LogWarning(
                    "Failed {RequestName} | User: {Username} | Error: {Error}",
                    requestName, _currentUser.Username, result.Error);
            }
            else
            {
                _logger.LogInformation(
                    "Completed {RequestName} | User: {Username} | Success",
                    requestName, _currentUser.Username);
            }
        }

        return response;
    }
}