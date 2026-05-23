using MediatR;
using TaskManagement.Application.Auth.DTOs;

namespace TaskManagement.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
