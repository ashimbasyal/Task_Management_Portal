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
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
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

                status.Name = request.Name;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Status updated successfully",
                    Data = new
                    {
                        status.Id,
                        status.Name
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update status",
                    Error = ex.Message
                };
            }
        }
    }
}
