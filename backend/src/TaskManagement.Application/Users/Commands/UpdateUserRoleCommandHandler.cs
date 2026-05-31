using MediatR;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserRoleCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserRoleCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        user.Role = (UserRole)request.Role;
        user.DepartmentId = request.Role == 3 ? request.DepartmentId : null;
        await userRepository.UpdateAsync(user, cancellationToken);

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
