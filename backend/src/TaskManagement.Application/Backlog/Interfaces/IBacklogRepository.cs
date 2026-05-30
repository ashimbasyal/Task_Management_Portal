using TaskManagement.Application.Backlog.Commands;

namespace TaskManagement.Application.Backlog.Interfaces;

public interface IBacklogRepository
{
    Task<IReadOnlyList<BacklogListItem>> GetAllAsync(int? departmentId = null, CancellationToken ct = default);
    Task<int> CreateAsync(CreateBacklogCommand command, CancellationToken ct = default);
    Task<int> MoveToSprintAsync(MoveToSprintCommand command, CancellationToken ct = default);
}

public record BacklogListItem(
    int Id,
    string Title,
    string? Description,
    string RequestedBy,
    string? GitLabLink,
    string? Remarks,
    string? Priority,
    string? Status,
    string? Department,
    bool IsMovedToSprint
);
