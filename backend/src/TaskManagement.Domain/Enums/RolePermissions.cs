using System.Collections.Frozen;

namespace TaskManagement.Domain.Enums;

public static class RolePermissions
{
    private static readonly FrozenDictionary<UserRole, FrozenSet<Permission>> _permissions;

    static RolePermissions()
    {
        var dict = new Dictionary<UserRole, FrozenSet<Permission>>
        {
            [UserRole.Admin] = new HashSet<Permission>
            {
                Permission.ViewUsers,
                Permission.CreateUsers,
                Permission.EditUsers,
                Permission.DeleteUsers,
                Permission.ManageUserPermissions,
                Permission.ViewBacklog,
                Permission.CreateBacklog,
                Permission.EditBacklog,
                Permission.DeleteBacklog,
                Permission.ViewSprint,
                Permission.AssignSprintTask,
                Permission.UpdateSprintProgress,
                Permission.UpdateSprintRemarks,
                Permission.UpdateSprintStatus,
                Permission.ViewDashboard,
                Permission.ViewMasterData,
                Permission.ManageMasterData,
                Permission.ViewAuditLogs,
            }.ToFrozenSet(),

            [UserRole.Developer] = new HashSet<Permission>
            {
                Permission.ViewSprint,
                Permission.UpdateSprintProgress,
                Permission.UpdateSprintRemarks,
                Permission.UpdateSprintStatus,
                Permission.ViewDashboard,
            }.ToFrozenSet(),

            [UserRole.Officer] = new HashSet<Permission>
            {
                Permission.ViewBacklog,
                Permission.CreateBacklog,
                Permission.EditBacklog,
                Permission.ViewDashboard,
            }.ToFrozenSet(),
        };

        _permissions = dict.ToFrozenDictionary();
    }

    public static FrozenSet<Permission> GetPermissions(UserRole role) =>
        _permissions.TryGetValue(role, out var perms) ? perms : FrozenSet<Permission>.Empty;
}
