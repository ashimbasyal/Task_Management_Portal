using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Authorization;
using TaskManagement.Application.Dashboard.Queries;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permission.ViewDashboard)]
    public async Task<IActionResult> Get(
        [FromQuery] string? sprint,
        [FromQuery] string? priority,
        [FromQuery] string? status,
        [FromQuery] string? department) =>
        Ok(await mediator.Send(new GetDashboardDataQuery(sprint, priority, status, department)));
}
