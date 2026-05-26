using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.AuditLogs.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class AuditLogRepository(AppDbContext db) : IAuditLogRepository
{
    public async Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken ct = default) =>
        await db.AuditLogs.OrderByDescending(a => a.ChangedAt).ToListAsync(ct);
}
