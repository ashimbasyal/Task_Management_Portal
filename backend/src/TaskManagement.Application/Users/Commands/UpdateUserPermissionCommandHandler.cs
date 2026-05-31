using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserPermissionCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<UpdateUserPermissionCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        if (user.Role != UserRole.Officer)
            throw new InvalidOperationException("Permission setting only applies to Officers.");

        user.CanViewAllDepartments = request.CanViewAllDepartments;
        await userManager.UpdateAsync(user);

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
