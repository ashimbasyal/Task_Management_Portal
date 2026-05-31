using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Priorities.command
{
    public class CreatePriorityCommandHandler : IRequestHandler<CreatePriorityCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreatePriorityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreatePriorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var priority = new Priority
                {
                    Name = request.Name
                };

                _context.Priorities.Add(priority);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Priority created successfully",
                    Data = priority
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create priority",
                    Error = ex.Message
                };
            }
        }
    }
}
