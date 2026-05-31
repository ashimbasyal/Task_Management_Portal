using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Authorization;
using TaskManagement.Application.Users.Commands;
using TaskManagement.Application.Users.Queries;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [RequirePermission(Permission.CreateUsers)]
    public async Task<IActionResult> Create(CreateUserCommand command) =>
        Ok(await mediator.Send(command));

    [HttpGet]
    [RequirePermission(Permission.ViewUsers)]
    public async Task<IActionResult> GetAll() =>
        Ok(await mediator.Send(new GetAllUsersQuery()));

    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewUsers)]
    public async Task<IActionResult> GetById(string id) =>
        Ok(await mediator.Send(new GetUserByIdQuery(id)));

    [HttpPatch("{id}/permission")]
    [RequirePermission(Permission.ManageUserPermissions)]
    public async Task<IActionResult> UpdatePermission(string id, [FromBody] UpdatePermissionRequest request) =>
        Ok(await mediator.Send(new UpdateUserPermissionCommand(id, request.CanViewAllDepartments)));

    [HttpPatch("{id}/role")]
    [RequirePermission(Permission.EditUsers)]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request) =>
        Ok(await mediator.Send(new UpdateUserRoleCommand(id, request.Role, request.DepartmentId)));

    [HttpGet("{id}/permissions")]
    [RequirePermission(Permission.ManageUserPermissions)]
    public async Task<IActionResult> GetPermissions(string id) =>
        Ok(await mediator.Send(new GetUserPermissionsQuery(id)));

    [HttpPut("{id}/permissions")]
    [RequirePermission(Permission.ManageUserPermissions)]
    public async Task<IActionResult> UpdatePermissions(string id, [FromBody] UpdatePermissionsRequest request) =>
        Ok(await mediator.Send(new UpdateUserPermissionsCommand(id, request.GrantedPermissions)));

    [HttpDelete("{id}")]
    [RequirePermission(Permission.DeleteUsers)]
    public async Task<IActionResult> Delete(string id)
    {
        await mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}

public record UpdatePermissionRequest(bool CanViewAllDepartments);
public record UpdateRoleRequest(int Role, int? DepartmentId);
public record UpdatePermissionsRequest(List<int> GrantedPermissions);
