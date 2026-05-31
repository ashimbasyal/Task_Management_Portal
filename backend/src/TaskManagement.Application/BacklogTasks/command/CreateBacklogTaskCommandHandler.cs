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
                var maxSN = await _context.BacklogTasks
                    .MaxAsync(x => (int?)x.SN, cancellationToken) ?? 0;

                var backlogTask = new BacklogTask
                {
                    SN = maxSN + 1,
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
                    StatusCode = 201,
                    Message = "Backlog task created successfully.",
                    Data = new
                    {
                        backlogTask.Id,
                        backlogTask.Title
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create backlog task.",
                    Error = ex.Message
                };
            }
        }
    }
}
