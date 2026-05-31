using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public GetDashboardQueryHandler(IApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<APIResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sprintWise = await _context.SprintTasks
                    .GroupBy(x => x.SprintName)
                    .Select(g => new SprintTaskCountDto
                    {
                        SprintName = g.Key,
                        TaskCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var statusWise = await _context.SprintTasks
                    .Include(x => x.Status)
                    .GroupBy(x => x.Status.Name)
                    .Select(g => new StatusDistributionDto
                    {
                        StatusName = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var priorityWise = await _context.SprintTasks
                    .Include(x => x.Priority)
                    .GroupBy(x => x.Priority.Name)
                    .Select(g => new PriorityDistributionDto
                    {
                        PriorityName = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var departmentWise = await _context.SprintTasks
                    .Include(x => x.Department)
                    .GroupBy(x => x.Department.Name)
                    .Select(g => new DepartmentDistributionDto
                    {
                        DepartmentName = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var userTasks = await _context.SprintTasks
                    .GroupBy(x => x.AssigneeId)
                    .Select(g => new UserTaskCountDto
                    {
                        UserId = g.Key,
                        TaskCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                // get users once (avoid N+1 problem)
                var users = await _userManager.Users.ToListAsync(cancellationToken);

                foreach (var u in userTasks)
                {
                    u.UserName = users.FirstOrDefault(x => x.Id == u.UserId)?.UserName;
                }

                var pending = await _context.SprintTasks
                    .CountAsync(x => x.Status.Name == "Pending", cancellationToken);

                var completed = await _context.SprintTasks
                    .CountAsync(x => x.Status.Name == "Completed", cancellationToken);

                var result = new DashboardResponseDto
                {
                    SprintWiseTaskCounts = sprintWise,
                    StatusWiseDistribution = statusWise,
                    PriorityWiseDistribution = priorityWise,
                    DepartmentWiseDistribution = departmentWise,
                    AssignedUserTaskCounts = userTasks,
                    PendingTasks = pending,
                    CompletedTasks = completed
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
