using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.SprintTasks.command;
using TaskManagement.Application.SprintTasks.Query;

namespace TaskManagement.API.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SprintTasksController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateSprintTask(CreateSprintTaskCommand command)
        {
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSprintTaskById(int id)
        {
            var response = await mediator.Send(
                new GetSprintTaskByIdQuery(id));

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSprintTask(UpdateSprintTaskCommand command)
        {
            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }
    }
}
