using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.AuditLogs.Command;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AuditLog(
           CreateAuditLogCommand command)
        {
            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }


    }
}
