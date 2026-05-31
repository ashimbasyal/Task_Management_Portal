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
    public class DeleteSprintTaskCommandHandler : IRequestHandler<DeleteSprintTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteSprintTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteSprintTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sprintTask = await _context.SprintTasks
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (sprintTask == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Sprint Task not found"
                    };
                }

                var backlogTask = await _context.BacklogTasks
                    .FirstOrDefaultAsync(
                        x => x.Id == sprintTask.BacklogTaskId,
                        cancellationToken);

                if (backlogTask != null)
                {
                    backlogTask.IsMovedToSprint = false;
                }

                _context.SprintTasks.Remove(sprintTask);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint Task deleted successfully"
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
