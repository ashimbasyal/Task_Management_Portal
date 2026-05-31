using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.AuditLogs.Command;
using TaskManagement.Application.BacklogTasks.command;
using TaskManagement.Application.BacklogTasks.Query;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BacklogTaskController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> BacklogTask(
          CreateBacklogTaskCommand command)
        {
            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBacklogTask(int id, UpdateBacklogTaskCommand command)
        {
            command.Id = id;

            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBacklogTaskById(int id)
        {
            var response = await mediator.Send(new GetBacklogTaskByIdQuery { Id = id });

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBacklogTask(int id)
        {
            var response = await mediator.Send(new DeleteBacklogTaskCommand { Id = id });

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBacklogTasks(
            [FromQuery] int? priorityId,
            [FromQuery] int? statusId,
            [FromQuery] int? departmentId)
        {
            var response = await mediator.Send(new GetAllBacklogTaskQuery
            {
                PriorityId = priorityId,
                StatusId = statusId,
                DepartmentId = departmentId
            });

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{id}/move-to-sprint")]
        public async Task<IActionResult> MoveToSprint(int id, [FromBody] MoveToSprintRequestDto request)
        {
            var command = new MoveToSprintCommand(id, request.SprintName, request.StartDate, request.EndDate, request.Remarks, request.AssigneeId);
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUploadBacklogTask(
        IFormFile file)
        {
            var response = await mediator.Send(
                new BulkUploadBacklogTaskCommand
                {
                    File = file
                });

            return StatusCode(response.StatusCode, response);
        }
    }

    public record MoveToSprintRequestDto(
        string SprintName,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Remarks,
        string? AssigneeId
    );
}
