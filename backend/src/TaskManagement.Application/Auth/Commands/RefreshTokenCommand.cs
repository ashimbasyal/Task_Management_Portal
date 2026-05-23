using MediatR;
using TaskManagement.Application.Auth.DTOs;

namespace TaskManagement.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
