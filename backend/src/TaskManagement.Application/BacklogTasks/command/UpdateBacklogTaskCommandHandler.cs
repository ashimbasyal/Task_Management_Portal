using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class UpdateBacklogTaskCommandHandler : IRequestHandler<UpdateBacklogTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateBacklogTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateBacklogTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var backlogTask = await _context.BacklogTasks
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (backlogTask == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Backlog task not found.",
                        Data = null,
                        Error = null
                    };
                }

                var oldStatusId = backlogTask.StatusId;
                backlogTask.Title = request.Title;
                backlogTask.Description = request.Description;
                backlogTask.RequestedBy = request.RequestedBy;
                backlogTask.GitLabLink = request.GitLabLink;
                backlogTask.Remarks = request.Remarks;
                backlogTask.PriorityId = request.PriorityId;
                backlogTask.StatusId = request.StatusId;
                backlogTask.DepartmentId = request.DepartmentId;
                backlogTask.UpdatedBy = request.UpdatedBy;
                backlogTask.UpdatedAt = DateTime.UtcNow;

                _context.BacklogTasks.Update(backlogTask);

                await _context.SaveChangesAsync(cancellationToken);

                if (!backlogTask.IsMovedToSprint && request.StatusId.HasValue
                    && oldStatusId != request.StatusId)
                {
                    var newStatus = await _context.Statuses
                        .FirstOrDefaultAsync(x => x.Id == request.StatusId, cancellationToken);

                    if (newStatus != null)
                    {
                        var triggerExists = await _context.SprintStatuses
                            .AnyAsync(x => x.Name == newStatus.Name && x.IsActive, cancellationToken);

                        if (triggerExists)
                        {
                            var sprintTask = new SprintTask
                            {
                                BacklogTaskId = backlogTask.Id,
                                SprintName = $"Sprint-{backlogTask.Id}",
                                StatusId = request.StatusId,
                                Remarks = backlogTask.Remarks,
                                CreatedAt = DateTime.UtcNow
                            };

                            _context.SprintTasks.Add(sprintTask);
                            backlogTask.IsMovedToSprint = true;
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Backlog task updated successfully.",
                    Data = backlogTask,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update backlog task.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
