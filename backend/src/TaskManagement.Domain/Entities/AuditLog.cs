namespace TaskManagement.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;   // CREATE | UPDATE | DELETE
    public int? RecordId { get; set; }
    public string? OldValues { get; set; }   // JSON snapshot
    public string? NewValues { get; set; }   // JSON snapshot
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
