using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Statuses.command
{
    public class CreateStatusCommandHandler : IRequestHandler<CreateStatusCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var status = new Status
                {
                    Name = request.Name
                };

                _context.Statuses.Add(status);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Status created successfully",
                    Data = status,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create status",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
