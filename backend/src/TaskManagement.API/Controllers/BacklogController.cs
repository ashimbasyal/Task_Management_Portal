using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Authorization;
using TaskManagement.Application.Backlog.Commands;
using TaskManagement.Application.Backlog.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BacklogController(IMediator mediator, IBacklogRepository repository) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permission.ViewBacklog)]
    public async Task<IActionResult> GetAll() =>
        Ok(await repository.GetAllAsync());

    [HttpPost]
    [RequirePermission(Permission.CreateBacklog)]
    public async Task<IActionResult> Create([FromBody] CreateBacklogRequest request)
    {
        var id = await mediator.Send(new CreateBacklogCommand(
            request.Title, request.Description, request.RequestedBy,
            request.GitLabLink, request.Remarks,
            request.Priority, request.Status, request.Department
        ));
        return Ok(new { id });
    }

    [HttpPost("{id}/move-to-sprint")]
    [RequirePermission(Permission.AssignSprintTask)]
    public async Task<IActionResult> MoveToSprint(int id, [FromBody] MoveToSprintRequest request)
    {
        var sprintId = await mediator.Send(new MoveToSprintCommand(
            id, request.SprintName, request.StartDate, request.EndDate,
            request.Remarks, request.AssigneeId
        ));
        return Ok(new { sprintId });
    }
}

public record MoveToSprintRequest(
    string SprintName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Remarks,
    int? AssigneeId
);

public record CreateBacklogRequest(
    string Title,
    string? Description,
    string RequestedBy,
    string? GitLabLink,
    string? Remarks,
    string Priority,
    string Status,
    string Department
);
