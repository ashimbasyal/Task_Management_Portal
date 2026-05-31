using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<BacklogTask> BacklogTasks { get; set; }
        = new List<BacklogTask>();

    public ICollection<AppUser> Users { get; set; } = [];
}
