using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintTasks.command
{
    public class UpdateSprintTaskCommandHandler : IRequestHandler<UpdateSprintTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateSprintTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateSprintTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sprintTask = await _context.SprintTasks
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

                var backlogTaskExists = await _context.BacklogTasks
                    .AnyAsync(
                        x => x.Id == request.BacklogTaskId,
                        cancellationToken);

                if (!backlogTaskExists)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Backlog task not found."
                    };
                }

                sprintTask.BacklogTaskId = request.BacklogTaskId;
                sprintTask.SprintName = request.SprintName;
                sprintTask.StartDate = request.StartDate;
                sprintTask.EndDate = request.EndDate;
                sprintTask.Remarks = request.Remarks;
                sprintTask.AssigneeId = request.AssigneeId;
                sprintTask.StatusId = request.StatusId;
                sprintTask.UpdatedAt = DateTime.UtcNow;
                sprintTask.UpdatedBy = request.UpdatedBy;

                _context.SprintTasks.Update(sprintTask);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint task updated successfully.",
                    Data = sprintTask
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
