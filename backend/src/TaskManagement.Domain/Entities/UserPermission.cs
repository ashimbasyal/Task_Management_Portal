using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class UserPermission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public Permission Permission { get; set; }
    public bool IsGranted { get; set; }
}
