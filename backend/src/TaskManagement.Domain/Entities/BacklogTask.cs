namespace TaskManagement.Domain.Entities;

public class BacklogTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? GitLabLink { get; set; }
    public string? Remarks { get; set; }

   
    public int? PriorityId { get; set; }
    public MasterData? Priority { get; set; }

    public int? StatusId { get; set; }
    public MasterData? Status { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public bool IsMovedToSprint { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public SprintTask? SprintTask { get; set; }
}
