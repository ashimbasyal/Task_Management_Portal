using Microsoft.AspNetCore.Authorization;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(Permission permission)
        : base(permission.ToString())
    {
    }
}
