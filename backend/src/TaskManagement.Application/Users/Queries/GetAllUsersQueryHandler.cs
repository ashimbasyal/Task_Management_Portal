using MediatR;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Application.Users.Interfaces;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(u => new UserDto(
            u.Id, u.FullName, u.Email!, u.Role,
            u.DepartmentId, u.Department?.Name, u.CanViewAllDepartments
        )).ToList();
    }
}
