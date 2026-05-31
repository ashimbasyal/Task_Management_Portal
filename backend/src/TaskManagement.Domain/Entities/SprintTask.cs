namespace TaskManagement.Domain.Entities;

public class SprintTask
{
    public int Id { get; set; }

    public int BacklogTaskId { get; set; }
    public BacklogTask BacklogTask { get; set; } = null!;

    public string SprintName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Remarks { get; set; }

   
    public int? AssigneeId { get; set; }
    public MasterData? Assignee { get; set; }

    
    public int? StatusId { get; set; }
    public Status? Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
