using MediatR;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Queries;

public record GetUserByIdQuery(string UserId) : IRequest<UserDto>;
