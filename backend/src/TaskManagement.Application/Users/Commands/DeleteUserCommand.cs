using MediatR;

namespace TaskManagement.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest;
