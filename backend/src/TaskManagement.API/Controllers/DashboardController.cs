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
           CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new GetDashboardQuery(),
                cancellationToken);

            return StatusCode(result.StatusCode, result);
        }
    }
}
