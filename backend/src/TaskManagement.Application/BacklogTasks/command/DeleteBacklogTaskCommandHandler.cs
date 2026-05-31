using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class DeleteBacklogTaskCommandHandler : IRequestHandler<DeleteBacklogTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteBacklogTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteBacklogTaskCommand request, CancellationToken cancellationToken)
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

                _context.BacklogTasks.Remove(backlogTask);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Backlog task deleted successfully.",
                    Data = backlogTask.Id,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to delete backlog task.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
