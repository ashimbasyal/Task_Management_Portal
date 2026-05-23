using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
}
