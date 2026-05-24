using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Users.Commands;
using TaskManagement.Application.Users.Queries;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    // Admin only: create a new user with a role.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateUserCommand command) =>
        Ok(await mediator.Send(command));

    // Admin only: list all users.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() =>
        Ok(await mediator.Send(new GetAllUsersQuery()));

    // Admin only: get a single user by ID.
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(string id) =>
        Ok(await mediator.Send(new GetUserByIdQuery(id)));

    //Admin only: update Officer's department visibility permission.
    [HttpPatch("{id}/permission")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePermission(string id, [FromBody] UpdatePermissionRequest request) =>
        Ok(await mediator.Send(new UpdateUserPermissionCommand(id, request.CanViewAllDepartments)));

    // Admin only: delete a user.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        await mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}

public record UpdatePermissionRequest(bool CanViewAllDepartments);
