using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Auth.Commands;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command) =>
        Ok(await mediator.Send(command));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command) =>
        Ok(await mediator.Send(command));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command) =>
        Ok(await mediator.Send(command));
}
