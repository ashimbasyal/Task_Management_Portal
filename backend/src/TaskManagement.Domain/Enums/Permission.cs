namespace TaskManagement.Domain.Enums;

public enum Permission
{
    // User management
    ViewUsers = 1,
    CreateUsers = 2,
    EditUsers = 3,
    DeleteUsers = 4,
    ManageUserPermissions = 5,

    // Backlog
    ViewBacklog = 10,
    CreateBacklog = 11,
    EditBacklog = 12,
    DeleteBacklog = 13,

    // Sprint
    ViewSprint = 20,
    CreateSprint = 21,
    AssignSprintTask = 22,
    UpdateSprintProgress = 23,
    UpdateSprintRemarks = 24,
    UpdateSprintStatus = 25,

    // Dashboard
    ViewDashboard = 30,

    // Master Data
    ViewMasterData = 40,
    ManageMasterData = 41,

    // Audit Logs
    ViewAuditLogs = 50,
}
