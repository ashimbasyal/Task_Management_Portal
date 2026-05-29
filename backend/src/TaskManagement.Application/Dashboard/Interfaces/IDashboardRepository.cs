using TaskManagement.Application.Dashboard.DTOs;

namespace TaskManagement.Application.Dashboard.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardDataDto> GetDashboardDataAsync(
        string? sprint = null,
        string? priority = null,
        string? status = null,
        string? department = null,
        CancellationToken ct = default);
}
