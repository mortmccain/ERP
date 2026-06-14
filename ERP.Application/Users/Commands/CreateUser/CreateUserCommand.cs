using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommand
{
    public string Username { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string Password { get; init; } = string.Empty;
    public string? Role { get; init; }

    // For permission checks
    public Guid RequestedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}