using ERP.Application.Common.Interfaces;
using ERP.Application.Users.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public UserRepository(UserManager<IdentityUser<Guid>> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var allUsers = await _userManager.Users.ToListAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var fullName = (await _userManager.GetClaimsAsync(user))
                .FirstOrDefault(c => c.Type == "FullName")?.Value;

            result.Add(new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = fullName,
                Roles = roles.ToList().AsReadOnly()
            });
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var fullName = (await _userManager.GetClaimsAsync(user))
            .FirstOrDefault(c => c.Type == "FullName")?.Value;

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = fullName,
            Roles = roles.ToList().AsReadOnly()
        };
    }

    public async Task<Guid> CreateAsync(string username, string? email, string? fullName, string password, string role, CancellationToken cancellationToken)
    {
        var user = new IdentityUser<Guid>
        {
            UserName = username,
            Email = email
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Role assignment failed: {errors}");
        }

        // Add full name claim
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            await _userManager.AddClaimAsync(user, new Claim("FullName", fullName));
        }

        return user.Id;
    }

    public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}