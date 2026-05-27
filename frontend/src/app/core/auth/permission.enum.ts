export enum Permission {
  ViewUsers = 'ViewUsers',
  CreateUsers = 'CreateUsers',
  EditUsers = 'EditUsers',
  DeleteUsers = 'DeleteUsers',
  ManageUserPermissions = 'ManageUserPermissions',

  ViewBacklog = 'ViewBacklog',
  CreateBacklog = 'CreateBacklog',
  EditBacklog = 'EditBacklog',
  DeleteBacklog = 'DeleteBacklog',

  ViewSprint = 'ViewSprint',
  AssignSprintTask = 'AssignSprintTask',
  UpdateSprintProgress = 'UpdateSprintProgress',
  UpdateSprintRemarks = 'UpdateSprintRemarks',
  UpdateSprintStatus = 'UpdateSprintStatus',

  ViewDashboard = 'ViewDashboard',

  ViewMasterData = 'ViewMasterData',
  ManageMasterData = 'ManageMasterData',

  ViewAuditLogs = 'ViewAuditLogs',
}
