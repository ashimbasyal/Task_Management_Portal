using MediatR;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Commands;

public record UpdateUserPermissionsCommand(string UserId, List<int> GrantedPermissions) : IRequest<List<string>>;
