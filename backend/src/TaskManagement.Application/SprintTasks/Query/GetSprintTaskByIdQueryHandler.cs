using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.SprintTasks.DTOs;

namespace TaskManagement.Application.SprintTasks.Query
{
    public class GetSprintTaskByIdQueryHandler : IRequestHandler<GetSprintTaskByIdQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetSprintTaskByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetSprintTaskByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.SprintTasks
                    .Include(x => x.Assignee)
                    .Include(x => x.Status)
                    .Include(x => x.BacklogTask)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (task == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Sprint Task not found"
                    };
                }

                var dto = new SprintTaskDto
                {
                    Id = task.Id,
                    BacklogTaskId = task.BacklogTaskId,
                    BacklogTaskSN = task.BacklogTask?.SN,
                    BacklogTaskTitle = task.BacklogTask?.Title,
                    SprintName = task.SprintName,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    Remarks = task.Remarks,
                    AssigneeId = task.AssigneeId,
                    AssigneeName = task.Assignee?.FullName,
                    StatusId = task.StatusId,
                    StatusName = task.Status?.Name,
                    PriorityName = task.BacklogTask != null && task.BacklogTask.Priority != null ? task.BacklogTask.Priority.Name : null,
                    CreatedAt = task.CreatedAt,
                    CreatedBy = task.CreatedBy,
                    UpdatedAt = task.UpdatedAt,
                    UpdatedBy = task.UpdatedBy
                };

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed",
                    Error = ex.Message
                };
            }
        }
    }
}
