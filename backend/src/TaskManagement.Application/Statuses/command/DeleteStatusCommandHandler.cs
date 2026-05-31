using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Statuses.command
{
    public class DeleteStatusCommandHandler : IRequestHandler<DeleteStatusCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _context.Statuses
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (status == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Status not found"
                    };
                }


                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Status deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to delete status",
                    Error = ex.Message
                };
            }
        }
    }
}
