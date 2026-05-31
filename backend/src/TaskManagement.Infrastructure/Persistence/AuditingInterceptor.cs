using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence;

public sealed class AuditingInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

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

            // Detect login: AppUser updated but only RefreshToken/RefreshTokenExpiry actually changed
            if (entry.Entity is AppUser appUser && entry.State == EntityState.Modified)
            {
                var changedProps = entry.Properties
                    .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                    .Select(p => p.Metadata.Name)
                    .ToHashSet();
                var loginOnlyProps = new HashSet<string>
                    { "RefreshToken", "RefreshTokenExpiry", "ConcurrencyStamp", "SecurityStamp" };
                if (changedProps.Count > 0 && changedProps.IsSubsetOf(loginOnlyProps))
                {
                    auditLogs.Add(new AuditLog
                    {
                        TableName = "AppUser",
                        Action = "LOGIN",
                        RecordId = null,
                        OldValues = null,
                        NewValues = JsonSerializer.Serialize(new Dictionary<string, object?>
                        {
                            ["FullName"] = appUser.FullName,
                            ["Email"] = appUser.Email
                        }, JsonOptions),
                        ChangedBy = userName,
                        ChangedAt = DateTime.UtcNow
                    });
                    continue;
                }
            }

            int? recordId = null;
            var idProperty = entry.Properties.FirstOrDefault(p =>
                string.Equals(p.Metadata.Name, "Id", StringComparison.OrdinalIgnoreCase));
            if (idProperty?.CurrentValue is int intVal)
                recordId = intVal;

            var sensitiveProps = new HashSet<string>
            {
                "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
                "RefreshToken", "RefreshTokenExpiry",
                "AccessFailedCount", "LockoutEnabled", "LockoutEnd",
                "TwoFactorEnabled", "PhoneNumberConfirmed",
                "NormalizedEmail", "NormalizedUserName",
                "EmailConfirmed", "PhoneNumber", "LockoutEnabled", "LockoutEnd"
            };

            // For AppUser updates, only include meaningful fields
            var allowedProps = entry.Entity is AppUser && entry.State is EntityState.Modified or EntityState.Added
                ? new HashSet<string> { "FullName", "Email", "UserName", "Role", "DepartmentId", "CanViewAllDepartments" }
                : null;

            string? oldValues = null;
            string? newValues = null;

            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                oldValues = SerializeValues(
                    entry.Properties
                        .Where(p => !sensitiveProps.Contains(p.Metadata.Name)
                            && (p.IsModified || entry.State == EntityState.Deleted)),
                    allowedProps,
                    useOriginal: true);
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                newValues = SerializeValues(
                    entry.Properties
                        .Where(p => !sensitiveProps.Contains(p.Metadata.Name)),
                    allowedProps);
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

    private static string SerializeValues(IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry> properties, HashSet<string>? allowedProps, bool useOriginal = false)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var p in properties)
        {
            if (allowedProps != null && !allowedProps.Contains(p.Metadata.Name))
                continue;

            var val = useOriginal ? p.OriginalValue : p.CurrentValue;
            if (val is Enum)
                val = val.ToString();

            dict[p.Metadata.Name] = val;
        }
        return JsonSerializer.Serialize(dict, JsonOptions);
    }
}
