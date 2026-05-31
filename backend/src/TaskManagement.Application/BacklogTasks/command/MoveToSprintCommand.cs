using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.command;

public record MoveToSprintCommand(
    int BacklogTaskId,
    string SprintName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Remarks,
    string? AssigneeId
) : IRequest<APIResponse>;
