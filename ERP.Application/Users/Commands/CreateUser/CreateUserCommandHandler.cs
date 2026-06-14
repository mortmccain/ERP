using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Common;

namespace ERP.Application.Users.Commands.CreateUser;

public static class CreateUserCommandHandler
{
    public static async Task<Result> Handle
        (
        CreateUserCommand command,
        IUserRepository userRepository,
        CancellationToken cancellationToken
        )
    {
        // Role enforcement
        string role;
        if (command.UserRoles.Contains("Admin"))
            role = string.IsNullOrWhiteSpace(command.Role) ? "Employee" : command.Role;
        else if (command.UserRoles.Contains("Manager"))
            role = "Employee";
        else
            return Result.Failure("Not authorized.");

        try
        {
            await userRepository.CreateAsync
                (
                command.Username,
                command.Email,
                command.FullName,
                command.Password,
                role,
                cancellationToken
                );
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }
}