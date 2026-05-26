using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.AuditLogs.Interfaces;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken ct = default);
}
