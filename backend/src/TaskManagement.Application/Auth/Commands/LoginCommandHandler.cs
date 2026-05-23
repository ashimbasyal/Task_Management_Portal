using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Auth.Common;
using TaskManagement.Application.Auth.DTOs;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth.Commands;

public sealed class LoginCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await TokenHelper.IssueTokensAsync(user, tokenService, userManager);
    }
}
