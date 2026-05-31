using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Dashboard.Query;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDashboard(
           [FromQuery] GetDashboardQuery query,
           CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                query,
                cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterDashboard(
           [FromBody] GetDashboardQuery query,
           CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                query,
                cancellationToken);

            return StatusCode(result.StatusCode, result);
        }
    }
}
