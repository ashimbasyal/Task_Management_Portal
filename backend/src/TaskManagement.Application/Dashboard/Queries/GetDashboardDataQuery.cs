using MediatR;
using TaskManagement.Application.Dashboard.DTOs;

namespace TaskManagement.Application.Dashboard.Queries;

public record GetDashboardDataQuery(
    string? Sprint = null,
    string? Priority = null,
    string? Status = null,
    string? Department = null
) : IRequest<DashboardDataDto>;
