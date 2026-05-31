namespace TaskManagement.Application.Dashboard.DTOs;

public record DashboardDataDto(
    int TotalTasks,
    int InProgress,
    int Completed,
    int ActiveSprints,
    List<ChartItemDto> PriorityDistribution,
    List<ChartItemDto> StatusDistribution,
    List<DepartmentItemDto> DepartmentDistribution,
    List<AssignedUserItemDto> AssignedUserCounts,
    List<SprintItemDto> SprintDistribution,
    int PendingTasks,
    int CompletedTasks
);

public record ChartItemDto(string Label, int Count, double Pct, string Color);

public record DepartmentItemDto(string Name, int Tasks, double Pct, string Color);

public record AssignedUserItemDto(string UserName, int TaskCount);

public record SprintItemDto(string SprintName, int TaskCount, int CompletedCount);
