using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }

    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    //view all or only  self 
    public bool CanViewAllDepartments { get; set; } = false;
}
