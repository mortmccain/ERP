using ERP.Application.Common.Interfaces;
using ERP.Application.Users.DTOs;
using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Queries.GetAllUsers;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllUsersAsync(cancellationToken);

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