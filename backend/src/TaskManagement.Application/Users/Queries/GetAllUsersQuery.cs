using MediatR;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Queries;

public record GetAllUsersQuery : IRequest<IReadOnlyList<UserDto>>;
