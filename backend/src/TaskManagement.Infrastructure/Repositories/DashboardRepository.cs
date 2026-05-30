using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Dashboard.DTOs;
using TaskManagement.Application.Dashboard.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class DashboardRepository(AppDbContext db) : IDashboardRepository
{
    private static readonly string[] Colors =
        ["#3b82f6", "#f59e0b", "#22c55e", "#ef4444", "#8b5cf6", "#14b8a6", "#f97316", "#6366f1"];

    public async Task<DashboardDataDto> GetDashboardDataAsync(
        string? sprint = null, string? priority = null, string? status = null, string? department = null,
        int? userDepartmentId = null,
        CancellationToken ct = default)
    {
        IQueryable<BacklogTask> backlogQuery = db.BacklogTasks.AsNoTracking()
            .Include(b => b.Priority)
            .Include(b => b.Status)
            .Include(b => b.Department);

        IQueryable<SprintTask> sprintQuery = db.SprintTasks.AsNoTracking()
            .Include(s => s.Status)
            .Include(s => s.BacklogTask).ThenInclude(b => b!.Priority)
            .Include(s => s.BacklogTask).ThenInclude(b => b!.Department);

        if (!string.IsNullOrWhiteSpace(priority))
        {
            backlogQuery = backlogQuery.Where(b => b.Priority!.Value == priority);
            sprintQuery = sprintQuery.Where(s => s.BacklogTask!.Priority!.Value == priority);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            backlogQuery = backlogQuery.Where(b => b.Status!.Value == status);
            sprintQuery = sprintQuery.Where(s => s.Status!.Value == status);
        }

        // Officer department visibility enforcement takes precedence
        var effectiveDepartment = userDepartmentId.HasValue
            ? (await db.Departments.FindAsync([userDepartmentId.Value], ct))?.Name
            : department;

        if (!string.IsNullOrWhiteSpace(effectiveDepartment))
            backlogQuery = backlogQuery.Where(b => b.Department!.Name == effectiveDepartment);

        if (!string.IsNullOrWhiteSpace(sprint))
            sprintQuery = sprintQuery.Where(s => s.SprintName == sprint);

        var backlogTasks = await backlogQuery.ToListAsync(ct);
        var sprintTasks = await sprintQuery.ToListAsync(ct);
        var masterData = await db.MasterData.AsNoTracking().ToListAsync(ct);
        var departments = await db.Departments.AsNoTracking().ToListAsync(ct);

        var statuses = masterData.Where(m => m.Type == MasterDataType.Status).ToList();
        var priorities = masterData.Where(m => m.Type == MasterDataType.Priority).ToList();
        var users = await db.Users.AsNoTracking().ToListAsync(ct);

        var totalTasks = backlogTasks.Count + sprintTasks.Count;
        var inProgress = backlogTasks.Count(b => b.Status?.Value == "In Progress") +
                         sprintTasks.Count(s => s.Status?.Value == "In Progress");
        var completed = backlogTasks.Count(b => b.Status?.Value == "Completed") +
                        sprintTasks.Count(s => s.Status?.Value == "Completed");
        var activeSprints = sprintTasks.Select(s => s.SprintName).Distinct().Count();

        // Priority distribution
        var priorityDistribution = priorities.Select(p =>
        {
            var count = backlogTasks.Count(b => b.PriorityId == p.Id) +
                        sprintTasks.Count(s => s.Status?.Value != null && s.BacklogTask?.PriorityId == p.Id);
            return new ChartItemDto(p.Value, count, 0, GetColor(p.Value));
        }).ToList();

        // Status distribution
        var statusDistribution = statuses.Select(s =>
        {
            var count = backlogTasks.Count(b => b.StatusId == s.Id) +
                        sprintTasks.Count(st => st.StatusId == s.Id);
            return new ChartItemDto(s.Value, count, 0, GetColor(s.Value));
        }).ToList();

        // Department distribution
        var deptDistribution = departments.Select(d =>
        {
            var count = backlogTasks.Count(b => b.DepartmentId == d.Id);
            return new DepartmentItemDto(d.Name, count, 0, GetColor(d.Name));
        }).ToList();

        // Assigned user counts (sprint tasks)
        var assignedUserCounts = sprintTasks
            .Where(s => s.AssigneeId != null)
            .GroupBy(s => s.Assignee?.Value ?? "Unknown")
            .Select(g => new AssignedUserItemDto(g.Key, g.Count()))
            .ToList();

        // Sprint distribution
        var sprintDistribution = sprintTasks
            .GroupBy(s => s.SprintName)
            .Select(g => new SprintItemDto(
                g.Key,
                g.Count(),
                g.Count(s => s.Status?.Value == "Completed")
            ))
            .ToList();

        // Pending vs Completed
        var pendingTasks = totalTasks - completed;
        var completedTasks = completed;

        // Calculate percentages
        var maxPriority = priorityDistribution.MaxBy(x => x.Count)?.Count ?? 1;
        var maxStatus = statusDistribution.MaxBy(x => x.Count)?.Count ?? 1;
        var maxDept = deptDistribution.MaxBy(x => x.Tasks)?.Tasks ?? 1;

        priorityDistribution = priorityDistribution.Select(p => p with { Pct = Math.Round((double)p.Count / maxPriority * 100, 1) }).ToList();
        statusDistribution = statusDistribution.Select(s => s with { Pct = Math.Round((double)s.Count / maxStatus * 100, 1) }).ToList();
        deptDistribution = deptDistribution.Select(d => d with { Pct = Math.Round((double)d.Tasks / maxDept * 100, 1) }).ToList();

        return new DashboardDataDto(
            totalTasks, inProgress, completed, activeSprints,
            priorityDistribution, statusDistribution, deptDistribution,
            assignedUserCounts, sprintDistribution, pendingTasks, completedTasks
        );
    }

    private static string GetColor(string key) =>
        key switch
        {
            "High" => "#ef4444",
            "Medium" => "#f59e0b",
            "Low" => "#22c55e",
            "Open" => "#3b82f6",
            "In Progress" => "#f59e0b",
            "Completed" => "#22c55e",
            "On Hold" => "#ef4444",
            _ => Colors[Math.Abs(key.GetHashCode()) % Colors.Length]
        };
}
