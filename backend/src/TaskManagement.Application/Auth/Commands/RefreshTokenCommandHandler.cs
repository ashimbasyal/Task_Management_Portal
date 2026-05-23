using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Auth.Common;
using TaskManagement.Application.Auth.DTOs;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth.Commands;

public sealed class RefreshTokenCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = userManager.Users.SingleOrDefault(u =>
            u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow)
            ?? throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        return await TokenHelper.IssueTokensAsync(user, tokenService, userManager);
    }
}
