using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options), IApplicationDbContext
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<MasterData> MasterData => Set<MasterData>();
    public DbSet<BacklogTask> BacklogTasks => Set<BacklogTask>();
    public DbSet<SprintTask> SprintTasks => Set<SprintTask>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

   

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

        // MasterData
        builder.Entity<MasterData>()
            .Property(m => m.Type)
            .HasConversion<int>();

        // BacklogTask → Priority (MasterData)
        builder.Entity<BacklogTask>()
            .HasOne(b => b.Priority)
            .WithMany()
            .HasForeignKey(b => b.PriorityId)
            .OnDelete(DeleteBehavior.SetNull);

        // BacklogTask → Status (MasterData)
        builder.Entity<BacklogTask>()
            .HasOne(b => b.Status)
            .WithMany()
            .HasForeignKey(b => b.StatusId)
            .OnDelete(DeleteBehavior.SetNull);

        // BacklogTask → Department
        builder.Entity<BacklogTask>()
            .HasOne(b => b.Department)
            .WithMany()
            .HasForeignKey(b => b.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // SprintTask → BacklogTask (1-to-1)
        builder.Entity<SprintTask>()
            .HasOne(s => s.BacklogTask)
            .WithOne(b => b.SprintTask)
            .HasForeignKey<SprintTask>(s => s.BacklogTaskId)
            .OnDelete(DeleteBehavior.Cascade);

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
    }
}
