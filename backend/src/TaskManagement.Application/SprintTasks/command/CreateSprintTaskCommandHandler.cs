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

                if (backlogTask.IsMovedToSprint)
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Task is moved to sprint."
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
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.SprintTasks.Add(sprintTask);

                backlogTask.IsMovedToSprint = true;

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
                    Message = "Failed to create sprint task.",
                    Error = ex.InnerException?.Message
                };
            }
        }
    }
}
