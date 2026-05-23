using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Auth.Common;
using TaskManagement.Application.Auth.DTOs;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth.Commands;

public sealed class RegisterCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return await TokenHelper.IssueTokensAsync(user, tokenService, userManager);
    }
}
