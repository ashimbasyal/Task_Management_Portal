using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.AuditLogs.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class AuditLogRepository(AppDbContext db) : IAuditLogRepository
{
    public async Task<IReadOnlyList<AuditLog>> GetAllAsync(
        string? tableName = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = BuildFilterQuery(tableName, action, from, to);

        return await query
            .OrderByDescending(a => a.ChangedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(
        string? tableName = null,
        string? action = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        return await BuildFilterQuery(tableName, action, from, to).CountAsync(ct);
    }

    private IQueryable<AuditLog> BuildFilterQuery(
        string? tableName, string? action, DateTime? from, DateTime? to)
    {
        var query = db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tableName))
            query = query.Where(a => a.TableName == tableName);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (from.HasValue)
            query = query.Where(a => a.ChangedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.ChangedAt <= to.Value);

        return query;
    }
}
