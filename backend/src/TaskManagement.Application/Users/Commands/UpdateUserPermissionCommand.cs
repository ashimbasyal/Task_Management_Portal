using MediatR;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Commands;


public record UpdateUserPermissionCommand(string UserId, bool CanViewAllDepartments) : IRequest<UserDto>;
