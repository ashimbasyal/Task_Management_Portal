using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Backlog.Commands;
using TaskManagement.Application.Backlog.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class BacklogRepository(AppDbContext db) : IBacklogRepository
{
    public async Task<IReadOnlyList<BacklogListItem>> GetAllAsync(int? departmentId = null, CancellationToken ct = default)
    {
        IQueryable<BacklogTask> query = db.BacklogTasks
            .AsNoTracking()
            .Include(b => b.Priority)
            .Include(b => b.Status)
            .Include(b => b.Department);

        if (departmentId.HasValue)
            query = query.Where(b => b.DepartmentId == departmentId.Value);

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BacklogListItem(
                b.Id,
                b.Title,
                b.Description,
                b.RequestedBy,
                b.GitLabLink,
                b.Remarks,
                b.Priority!.Value,
                b.Status!.Value,
                b.Department!.Name,
                b.IsMovedToSprint
            ))
            .ToListAsync(ct);
    }

    public async Task<int> CreateAsync(CreateBacklogCommand command, CancellationToken ct = default)
    {
        var priority = await db.MasterData
            .FirstOrDefaultAsync(m => m.Type == MasterDataType.Priority && m.Value == command.Priority, ct);
        var status = await db.MasterData
            .FirstOrDefaultAsync(m => m.Type == MasterDataType.Status && m.Value == command.Status, ct);
        var department = await db.Departments
            .FirstOrDefaultAsync(d => d.Name == command.Department, ct);

        var backlogTask = new BacklogTask
        {
            Title = command.Title,
            Description = command.Description,
            RequestedBy = command.RequestedBy,
            GitLabLink = command.GitLabLink,
            Remarks = command.Remarks,
            PriorityId = priority?.Id,
            StatusId = status?.Id,
            DepartmentId = department?.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.BacklogTasks.Add(backlogTask);
        await db.SaveChangesAsync(ct);

        return backlogTask.Id;
    }

    public async Task<int> MoveToSprintAsync(MoveToSprintCommand command, CancellationToken ct = default)
    {
        var backlogTask = await db.BacklogTasks
            .Include(b => b.Priority)
            .Include(b => b.Status)
            .FirstOrDefaultAsync(b => b.Id == command.BacklogTaskId, ct);

        if (backlogTask is null)
            throw new KeyNotFoundException($"Backlog task {command.BacklogTaskId} not found");

        if (backlogTask.IsMovedToSprint)
            throw new InvalidOperationException($"Backlog task {command.BacklogTaskId} already moved to sprint");

        // validate assignee exists in MasterData before setting FK
        var assigneeId = command.AssigneeId.HasValue
            ? await db.MasterData.AnyAsync(m => m.Id == command.AssigneeId.Value, ct)
                ? command.AssigneeId
                : null
            : null;

        var sprintTask = new SprintTask
        {
            BacklogTaskId = command.BacklogTaskId,
            SprintName = command.SprintName,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Remarks = command.Remarks,
            AssigneeId = assigneeId,
            StatusId = backlogTask.StatusId,
            CreatedAt = DateTime.UtcNow
        };

        backlogTask.IsMovedToSprint = true;
        backlogTask.UpdatedAt = DateTime.UtcNow;

        db.SprintTasks.Add(sprintTask);
        await db.SaveChangesAsync(ct);

        return sprintTask.Id;
    }
}
