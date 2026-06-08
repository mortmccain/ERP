using ERP.Application.Common.Interfaces;
using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
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
            await _userRepository.CreateAsync(
                command.Username,
                command.Email,
                command.FullName,
                command.Password,
                role,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        return Result.Success();
    }
}