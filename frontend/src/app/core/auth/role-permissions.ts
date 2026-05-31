import { Permission } from './permission.enum';

export const RolePermissions: Record<number, Permission[]> = {
  1: [
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
  ],
  2: [
    Permission.ViewSprint,
    Permission.UpdateSprintProgress,
    Permission.UpdateSprintRemarks,
    Permission.UpdateSprintStatus,
    Permission.ViewDashboard,
  ],
   3: [
    Permission.ViewUsers,
    Permission.ViewBacklog,
    Permission.CreateBacklog,
    Permission.EditBacklog,
    Permission.ViewDashboard,
  ],
};
