using System.Security.Claims;
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
        [FromQuery] string? department)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        int? userDepartmentId = null;

        if (role == UserRole.Officer.ToString())
        {
            var canViewAll = User.FindFirstValue("CanViewAllDepartments") == "True";
            if (!canViewAll)
            {
                var deptClaim = User.FindFirstValue("DepartmentId");
                if (int.TryParse(deptClaim, out var deptId))
                    userDepartmentId = deptId;
            }
        }

        return Ok(await mediator.Send(
            new GetDashboardDataQuery(sprint, priority, status, department, userDepartmentId)));
    }
}
