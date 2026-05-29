using MediatR;

namespace TaskManagement.Application.Backlog.Commands;

public record CreateBacklogCommand(
    string Title,
    string? Description,
    string RequestedBy,
    string? GitLabLink,
    string? Remarks,
    string Priority,
    string Status,
    string Department
) : IRequest<int>;
