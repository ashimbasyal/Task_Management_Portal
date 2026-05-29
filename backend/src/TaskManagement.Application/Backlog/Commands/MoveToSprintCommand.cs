using MediatR;

namespace TaskManagement.Application.Backlog.Commands;

public record MoveToSprintCommand(
    int BacklogTaskId,
    string SprintName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Remarks,
    int? AssigneeId
) : IRequest<int>;
