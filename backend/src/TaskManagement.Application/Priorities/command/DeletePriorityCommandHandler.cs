using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Priorities.command
{
    public class DeletePriorityCommandHandler : IRequestHandler<DeletePriorityCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeletePriorityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeletePriorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var priority = await _context.Priorities
                    .FirstOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

                if (priority == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Priority not found"
                    };
                }

                priority.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Priority deactivated successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to delete priority",
                    Error = ex.Message
                };
            }
        }
    }
}
