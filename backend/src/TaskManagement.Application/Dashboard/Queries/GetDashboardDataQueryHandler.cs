using MediatR;
using TaskManagement.Application.Dashboard.DTOs;
using TaskManagement.Application.Dashboard.Interfaces;

namespace TaskManagement.Application.Dashboard.Queries;

public sealed class GetDashboardDataQueryHandler(IDashboardRepository repository)
    : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
{
    public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken) =>
        await repository.GetDashboardDataAsync(
            request.Sprint, request.Priority, request.Status, request.Department, cancellationToken);
}
