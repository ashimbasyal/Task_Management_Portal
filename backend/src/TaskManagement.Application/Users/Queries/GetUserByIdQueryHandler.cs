using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetUserByIdQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
