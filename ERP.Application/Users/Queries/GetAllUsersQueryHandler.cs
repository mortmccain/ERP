using ERP.Application.Common.Interfaces;
using ERP.Application.Users.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Queries.GetAllUsers;

public static class GetAllUsersQueryHandler
{
    public static async Task<Result<List<UserDto>>> Handle
        (
        GetAllUsersQuery query,
        IUserRepository userRepository,
        CancellationToken cancellationToken
        )
    {
        var allUsers = await userRepository.GetAllUsersAsync(cancellationToken);

        var isAdmin = query.UserRoles.Contains("Admin");
        var isManager = query.UserRoles.Contains("Manager");

        var filteredUsers = allUsers.Where(u =>
        {
            if (isAdmin) return true;
            if (isManager) return u.Roles.Contains("Employee");
            return false;
        }).ToList();

        return Result<List<UserDto>>.Success(filteredUsers);
    }
}