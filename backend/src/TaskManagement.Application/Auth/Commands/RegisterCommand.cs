using MediatR;
using TaskManagement.Application.Auth.DTOs;

namespace TaskManagement.Application.Auth.Commands;

public record RegisterCommand(string Email, string Password) : IRequest<AuthResponse>;
