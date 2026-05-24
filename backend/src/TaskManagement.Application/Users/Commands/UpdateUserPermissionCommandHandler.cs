using MediatR;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserPermissionCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserPermissionCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        if (user.Role != UserRole.Officer)
            throw new InvalidOperationException("Permission setting only applies to Officers.");

        user.CanViewAllDepartments = request.CanViewAllDepartments;
        await userRepository.UpdateAsync(user, cancellationToken);

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
