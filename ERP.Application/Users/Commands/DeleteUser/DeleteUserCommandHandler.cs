using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Commands.DeleteUser;

public static class DeleteUserCommandHandler
{
    public static async Task<Result> Handle
        (
        DeleteUserCommand command,
        IUserRepository userRepository,
        CancellationToken cancellationToken
        )
    {
        if (command.UserId == command.RequestedByUserId)
            return Result.Failure("Cannot delete yourself.");

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        // Manager guard (if needed — you could also pass user roles here)
        if (command.UserRoles.Contains("Manager") && !command.UserRoles.Contains("Admin"))
        {
            if (!user.Roles.Contains("Employee"))
                return Result.Failure("Managers can only delete employees.");
        }

        var success = await userRepository.DeleteAsync(command.UserId, cancellationToken);
        if (!success)
            return Result.Failure("Delete failed.");

        return Result.Success();
    }
}