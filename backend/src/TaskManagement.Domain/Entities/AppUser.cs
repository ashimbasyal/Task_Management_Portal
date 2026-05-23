using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}
