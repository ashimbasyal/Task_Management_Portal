using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.Dashboard.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Dashboard.Query
{
    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public GetDashboardQueryHandler(
            IApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<APIResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var query = _context.SprintTasks.AsQueryable();

                
                if (!string.IsNullOrEmpty(request.SprintName))
                    query = query.Where(x => x.SprintName == request.SprintName);

                if (request.PriorityId.HasValue)
                    query = query.Where(x => x.PriorityId == request.PriorityId.Value);

                if (request.StatusId.HasValue)
                    query = query.Where(x => x.StatusId == request.StatusId.Value);

                if (request.DepartmentId.HasValue)
                    query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

                var totalTasks = await query.CountAsync(cancellationToken);

                var inProgressTasks = await query.CountAsync(x => x.StatusId == 2, cancellationToken);

                var pendingTasks = await query.CountAsync(x => x.Status.Name == "Pending", cancellationToken);

                var completedTasks = await query.CountAsync(x => x.Status.Name == "Completed", cancellationToken);

               

                var sprintWise = await query
                    .GroupBy(x => x.SprintName)
                    .Select(g => new SprintTaskCountDto
                    {
                        SprintName = g.Key,
                        TaskCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var statusWise = await query
                    .Include(x => x.Status)
                    .GroupBy(x => x.StatusId)
                    .Select(g => new StatusDistributionDto
                    {
                        StatusName = g.Min(x => x.Status.Name) ?? "Unassigned",
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var priorityWise = await query
                    .Include(x => x.Priority)
                    .GroupBy(x => x.PriorityId)
                    .Select(g => new PriorityDistributionDto
                    {
                        PriorityName = g.Min(x => x.Priority.Name) ?? "Unassigned",
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var departmentWise = await query
                    .Include(x => x.Department)
                    .GroupBy(x => x.DepartmentId)
                    .Select(g => new DepartmentDistributionDto
                    {
                        DepartmentName = g.Min(x => x.Department.Name) ?? "Unassigned",
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var userTasks = await query
                    .GroupBy(x => x.AssigneeId)
                    .Select(g => new UserTaskCountDto
                    {
                        UserId = g.Key,
                        TaskCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var users = await _userManager.Users
                    .Select(x => new { x.Id, x.UserName })
                    .ToListAsync(cancellationToken);

                foreach (var item in userTasks)
                {
                    item.UserName = users.FirstOrDefault(x => x.Id == item.UserId)?.UserName;
                }

                var result = new DashboardResponseDto
                {
                    SprintWiseTaskCounts = sprintWise,
                    StatusWiseDistribution = statusWise,
                    PriorityWiseDistribution = priorityWise,
                    DepartmentWiseDistribution = departmentWise,
                    AssignedUserTaskCounts = userTasks,

                    TotalTasks = totalTasks,
                    InProgressTasks = inProgressTasks,
                    PendingTasks = pendingTasks,
                    CompletedTasks = completedTasks
                };

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Dashboard data fetched successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to fetch dashboard data",
                    Error = ex.Message
                };
            }
        }
    }
}