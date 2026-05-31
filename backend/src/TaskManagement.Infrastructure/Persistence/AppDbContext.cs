using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;
namespace TaskManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options), IApplicationDbContext
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<BacklogTask> BacklogTasks => Set<BacklogTask>();
    public DbSet<SprintTask> SprintTasks => Set<SprintTask>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Priority> Priorities => Set<Priority>();

    public DbSet<SprintStatusTrigger> SprintStatuses => Set<SprintStatusTrigger>();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AppUser → Department
        builder.Entity<AppUser>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<AppUser>()
            .Property(u => u.Role)
            .HasConversion<int>();

        // BacklogTask → Priority (MasterData)
        builder.Entity<BacklogTask>()
     .HasOne(b => b.Priority)
     .WithMany(p => p.BacklogTasks)
     .HasForeignKey(b => b.PriorityId)
     .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<BacklogTask>()
    .HasIndex(b => b.Title)
    .IsUnique();

        builder.Entity<BacklogTask>()
            .HasOne(b => b.Status)
            .WithMany(s => s.BacklogTasks)
            .HasForeignKey(b => b.StatusId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<BacklogTask>()
            .HasOne(b => b.Department)
            .WithMany(d => d.BacklogTasks)
            .HasForeignKey(b => b.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // SprintTask → Assignee (MasterData)
        builder.Entity<SprintTask>()
             .HasOne(s => s.Assignee)
             .WithMany()
             .HasForeignKey(s => s.AssigneeId)
             .OnDelete(DeleteBehavior.SetNull);

        // SprintTask → Status (MasterData)
        builder.Entity<SprintTask>()
            .HasOne(s => s.Status)
            .WithMany()
            .HasForeignKey(s => s.StatusId)
            .OnDelete(DeleteBehavior.SetNull);

        // UserPermission → AppUser
        builder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserPermission>()
            .Property(up => up.Permission)
            .HasConversion<int>();

        builder.Entity<UserPermission>()
            .HasIndex(up => new { up.UserId, up.Permission })
            .IsUnique();
    }
}
