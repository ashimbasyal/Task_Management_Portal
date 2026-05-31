using MediatR;

namespace TaskManagement.Application.BacklogTasks.command;

public record MoveToSprintCommand(
    int BacklogTaskId,
    string SprintName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Remarks,
    int? AssigneeId
) : IRequest<int>;
