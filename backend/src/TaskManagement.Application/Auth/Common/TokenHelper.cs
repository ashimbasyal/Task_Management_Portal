using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Auth.DTOs;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth.Common;

/// <summary>Shared logic: issue new tokens and persist refresh token on the user.</summary>
internal static class TokenHelper
{
    internal static async Task<AuthResponse> IssueTokensAsync(
        AppUser user, ITokenService tokenService, UserManager<AppUser> userManager)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);
        return new AuthResponse(tokenService.GenerateAccessToken(user), refreshToken);
    }
}
