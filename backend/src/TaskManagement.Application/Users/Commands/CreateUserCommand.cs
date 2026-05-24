using MediatR;
using TaskManagement.Application.Users.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    int? DepartmentId
) : IRequest<UserDto>;
