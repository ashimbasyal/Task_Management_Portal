using MediatR;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Application.Users.Interfaces;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        return new UserDto(user.Id, user.FullName, user.Email!, user.Role,
            user.DepartmentId, user.Department?.Name, user.CanViewAllDepartments);
    }
}
