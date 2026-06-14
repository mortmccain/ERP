using ERP.Application.Users.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Queries.GetAllUsers;

public sealed class GetAllUsersQuery : IRequest<Result<List<UserDto>>>
{
    public Guid RequestedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}