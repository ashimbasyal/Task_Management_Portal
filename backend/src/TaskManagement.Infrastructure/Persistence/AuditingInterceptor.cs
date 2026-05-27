using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

public sealed class AuditingInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditLog)
                continue;

            var tableName = entry.Entity.GetType().Name;
            var action = entry.State switch
            {
                EntityState.Added => "CREATE",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted => "DELETE",
                _ => throw new InvalidOperationException()
            };

            int? recordId = null;
            var idProperty = entry.Properties.FirstOrDefault(p =>
                string.Equals(p.Metadata.Name, "Id", StringComparison.OrdinalIgnoreCase));
            if (idProperty?.CurrentValue is int intVal)
                recordId = intVal;

            string? oldValues = null;
            string? newValues = null;

            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                oldValues = JsonSerializer.Serialize(
                    entry.Properties.Where(p => p.IsModified || entry.State == EntityState.Deleted)
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                newValues = JsonSerializer.Serialize(
                    entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
            }

            auditLogs.Add(new AuditLog
            {
                TableName = tableName,
                Action = action,
                RecordId = recordId,
                OldValues = oldValues,
                NewValues = newValues,
                ChangedBy = userName,
                ChangedAt = DateTime.UtcNow
            });
        }

        if (auditLogs.Count > 0)
            context.Set<AuditLog>().AddRange(auditLogs);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
