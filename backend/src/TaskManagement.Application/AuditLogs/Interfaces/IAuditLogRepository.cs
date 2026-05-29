using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.AuditLogs.Interfaces;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetAllAsync(
        string? tableName = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<int> GetTotalCountAsync(
        string? tableName = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default);
}
