using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.SprintTasks.command
{
    public class CreateSprintTaskCommandHandler : IRequestHandler<CreateSprintTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateSprintTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateSprintTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var backlogTask = await _context.BacklogTasks
                   .FirstOrDefaultAsync(
                       x => x.Id == request.BacklogTaskId,
                       cancellationToken);

                if (backlogTask == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Backlog task not found."
                    };
                }

                
                var existingSprintTask = await _context.SprintTasks
                    .FirstOrDefaultAsync(
                        x => x.BacklogTaskId == request.BacklogTaskId,
                        cancellationToken);

                if (existingSprintTask != null)
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Sprint task already exists for this backlog task."
                    };
                }

                var sprintTask = new SprintTask
                {
                    BacklogTaskId = request.BacklogTaskId,
                    SprintName = request.SprintName,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Remarks = request.Remarks,
                    AssigneeId = request.AssigneeId,
                    StatusId = request.StatusId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                };

                await _context.SprintTasks.AddAsync(
                    sprintTask,
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Sprint task created successfully.",
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
