using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public Guid RequestedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}