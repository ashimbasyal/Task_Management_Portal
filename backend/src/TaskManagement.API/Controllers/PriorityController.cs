using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Priorities.command;
using TaskManagement.Application.Priorities.Query;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PriorityController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreatePriorityCommand command)
        {
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await mediator.Send(new GetPrioritiesQuery());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePriorityCommand command)
        {
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await mediator.Send(new DeletePriorityCommand { ID = id });
            return StatusCode(response.StatusCode, response);
        }
    }
}
