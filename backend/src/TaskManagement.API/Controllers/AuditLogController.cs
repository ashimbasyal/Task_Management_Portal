using System.Reflection.Metadata.Ecma335;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.AuditLogs.Command;
using TaskManagement.Application.AuditLogs.Query;
using TaskManagement.Application.Auth.Commands;

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

        [HttpGet]
        public async Task<IActionResult> GetAuditLog()
        {
            var response = await mediator.Send(
                new GetAuditLogQuery());

            return StatusCode(response.StatusCode, response);
        }
    }
}
