using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Services;

public class TokenService(IConfiguration config, UserManager<AppUser> userManager, AppDbContext db) : ITokenService
{
    public string GenerateAccessToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var permissions = RolePermissions.GetPermissions(user.Role);

        // apply user-level permission overrides
        var overrides = db.UserPermissions
            .Where(up => up.UserId == user.Id)
            .ToLookup(up => up.Permission, up => up.IsGranted);

        foreach (var permission in permissions)
        {
            if (!overrides.Contains(permission) || overrides[permission].First())
                claims.Add(new Claim("Permission", permission.ToString()));
        }

        // add extra granted permissions not in the role
        foreach (var group in overrides)
        {
            if (!permissions.Contains(group.Key) && group.First())
                claims.Add(new Claim("Permission", group.Key.ToString()));
        }

        if (user.DepartmentId.HasValue)
        {
            claims.Add(new Claim("DepartmentId", user.DepartmentId.Value.ToString()));
        }

        claims.Add(new Claim("CanViewAllDepartments", user.CanViewAllDepartments.ToString()));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
