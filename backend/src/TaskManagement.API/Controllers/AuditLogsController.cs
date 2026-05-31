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
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tableName,
        [FromQuery] string? action,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50) =>
        Ok(await mediator.Send(new GetAuditLogsQuery(tableName, action, from, to, page, pageSize)));
}
