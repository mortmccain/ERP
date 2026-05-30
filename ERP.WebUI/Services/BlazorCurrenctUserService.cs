using System.Security.Claims;
using ERP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace ERP.WebUI.Services;

/// <summary>
/// Blazor Server implementation of ICurrentUserService.
/// Extracts user information from the Blazor AuthenticationStateProvider.
/// </summary>
public sealed class BlazorCurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    // ↑ This is Blazor's built-in "who's logged in" tracker It knows about the authentication cookie and the current user

    public BlazorCurrentUserService(AuthenticationStateProvider authStateProvider)
    {
        // configuring the service to provide the user's identity in real-time and tracks it
        _authStateProvider = authStateProvider;
    }

    public Guid UserId
    {
        get
        {
            var user = GetUser();
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim is not null ? Guid.Parse(userIdClaim) : Guid.Empty;
        }
    }

    public string Username
    {
        get
        {
            var user = GetUser();
            return user.Identity?.Name ?? string.Empty;
        }
    }

    public string FullName
    {
        get
        {
            var user = GetUser();
            return user.FindFirst("FullName")?.Value ?? string.Empty;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = GetUser();
            return user.Identity?.IsAuthenticated ?? false;
        }
    }

    /// <summary>
    /// Synchronously retrieves the current ClaimsPrincipal.
    /// In Blazor Server, the AuthenticationState is available synchronously
    /// within the SignalR circuit's scope.
    /// </summary>
    private ClaimsPrincipal GetUser()
    {
        // This is synchronous because in Blazor Server,
        // the authentication state is already established for the circuit.
        var authState = _authStateProvider.GetAuthenticationStateAsync();

        // Result means I know it's already available. Give it to me NOW (synchronously)
        return authState.Result.User;

        /*
         Why .Result? In Blazor Server, the authentication state is established when the SignalR circuit is created.
         It's already there. It's not a database call. So .Result (synchronous) is safe here.
         */
    }
}