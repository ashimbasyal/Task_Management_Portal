using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.SprintTasks.DTOs;

namespace TaskManagement.Application.SprintTasks.Query
{
    public class GetAllSprintTasksQueryHandler : IRequestHandler<GetAllSprintTasksQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetAllSprintTasksQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetAllSprintTasksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sprintTasks = await _context.SprintTasks
                    .Include(x => x.Assignee)
                    .Include(x => x.Status)
                    .Include(x => x.BacklogTask)
                    .Select(x => new SprintTaskDto
                    {
                        Id = x.Id,
                        BacklogTaskId = x.BacklogTaskId,
                        BacklogTaskSN = x.BacklogTask.SN,
                        BacklogTaskTitle = x.BacklogTask.Title,
                        SprintName = x.SprintName,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Remarks = x.Remarks,
                        AssigneeId = x.AssigneeId,
                        AssigneeName = x.Assignee != null ? x.Assignee.FullName : null,
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : null,
                        PriorityName = x.BacklogTask.Priority != null ? x.BacklogTask.Priority.Name : null,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint tasks retrieved successfully.",
                    Data = sprintTasks
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve sprint tasks.",
                    Error = ex.Message
                };
            }
        }
    }
}
