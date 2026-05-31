using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetAllUsersQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .Include(u => u.Department)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id, u.FullName, u.Email!, u.Role,
            u.DepartmentId, u.Department?.Name, u.CanViewAllDepartments
        )).ToList();
    }
}
