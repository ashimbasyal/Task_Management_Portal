using MediatR;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Commands;

public record UpdateUserRoleCommand(string UserId, int Role, int? DepartmentId) : IRequest<UserDto>;
