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
    public class UpdatePriorityCommandHandler : IRequestHandler<UpdatePriorityCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdatePriorityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdatePriorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var priority = await _context.Priorities
                    .FirstOrDefaultAsync(x => x.ID == request.Id, cancellationToken);

                if (priority == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Priority not found"
                    };
                }

                priority.Name = request.Name;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Priority updated successfully",
                    Data = new
                    {
                        priority.ID,
                        priority.Name
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update priority",
                    Error = ex.Message
                };
            }
        }
    }
}
