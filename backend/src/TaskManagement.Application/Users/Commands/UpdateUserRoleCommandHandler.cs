using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserRoleCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<UpdateUserRoleCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        user.Role = (UserRole)request.Role;
        user.DepartmentId = request.Role == 3 ? request.DepartmentId : null;
        await userManager.UpdateAsync(user);

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
