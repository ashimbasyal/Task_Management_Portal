using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Authorization;
using TaskManagement.Application.AuditLogs.Queries;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permission.ViewAuditLogs)]
    public async Task<IActionResult> GetAll() =>
        Ok(await mediator.Send(new GetAuditLogsQuery()));
}
