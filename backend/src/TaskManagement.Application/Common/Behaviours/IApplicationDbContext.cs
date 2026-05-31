using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Common.Behaviours
{
    public interface IApplicationDbContext
    {
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<BacklogTask> BacklogTasks { get; }
        DbSet<SprintTask> SprintTasks { get; }
        DbSet<Status> Statuses { get; }
        DbSet<Priority> Priorities { get; }
        DbSet<SprintStatusTrigger> SprintStatuses { get; }
        DbSet<Department> Departments { get; }
        DbSet<AppUser> Users { get; }
        DbSet<UserPermission> UserPermissions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
