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
                var sprintTask = await _context.SprintTasks
                    .Include(x => x.Assignee)
                    .Include(x => x.Status)
                    .FirstOrDefaultAsync(
                        x => x.Id == request.Id,
                        cancellationToken);

                if (sprintTask == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Sprint task not found."
                    };
                }

                var dto = new SprintTaskDto
                {
                    Id = sprintTask.Id,
                    BacklogTaskId = sprintTask.BacklogTaskId,
                    SprintName = sprintTask.SprintName,
                    StartDate = sprintTask.StartDate,
                    EndDate = sprintTask.EndDate,
                    Remarks = sprintTask.Remarks,
                    AssigneeId = sprintTask.AssigneeId,
                    AssigneeName = sprintTask.Assignee != null
                        ? sprintTask.Assignee.Type.ToString()
                        : null,
                    StatusId = sprintTask.StatusId,
                    CreatedAt = sprintTask.CreatedAt,
                    CreatedBy = sprintTask.CreatedBy,
                    UpdatedAt = sprintTask.UpdatedAt,
                    UpdatedBy = sprintTask.UpdatedBy
                };

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint task fetched successfully.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}
