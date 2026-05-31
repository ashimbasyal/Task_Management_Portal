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
                var task = await _context.SprintTasks
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (task == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Sprint Task not found"
                    };
                }

                task.SprintName = request.SprintName!;
                task.StartDate = request.StartDate;
                task.EndDate = request.EndDate;
                task.Remarks = request.Remarks;
                task.AssigneeId = request.AssigneeId;
                task.StatusId = request.StatusId;
                task.UpdatedBy = request.UpdatedBy;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint Task updated successfully"
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
