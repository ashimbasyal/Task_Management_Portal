using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class CreateUserCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            DepartmentId = request.Role == UserRole.Officer ? request.DepartmentId : null,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, request.Role.ToString());

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role, user.DepartmentId, null, user.CanViewAllDepartments);
    }
}
