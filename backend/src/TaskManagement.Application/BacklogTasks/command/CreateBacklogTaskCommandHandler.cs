using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class CreateBacklogTaskCommandHandler : IRequestHandler<CreateBacklogTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;

        public CreateBacklogTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async  Task<APIResponse> Handle(CreateBacklogTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var backlogTask = new BacklogTask
                {
                    Title = request.Title,
                    Description = request.Description,
                    RequestedBy = request.RequestedBy,
                    GitLabLink = request.GitLabLink,
                    Remarks = request.Remarks,
                    PriorityId = request.PriorityId,
                    StatusId = request.StatusId,
                    DepartmentId = request.DepartmentId,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsMovedToSprint = false
                };

                _context.BacklogTasks.Add(backlogTask);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Backlog task created successfully.",
                    Data = backlogTask,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create backlog task.",
                    Data = null,
                    Error = ex.Message
                };
            }
        
        }
    }
}
