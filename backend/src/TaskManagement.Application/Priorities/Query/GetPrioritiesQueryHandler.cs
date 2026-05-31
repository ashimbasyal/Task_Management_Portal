using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.Priorities.DTOs;

namespace TaskManagement.Application.Priorities.Query
{
    public class GetPrioritiesQueryHandler : IRequestHandler<GetPrioritiesQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetPrioritiesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetPrioritiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var priorities = await _context.Priorities
                    .Where(x => x.IsActive)
                    .Select(x => new PriorityDto
                    {
                        Id = x.ID,
                        Name = x.Name
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Priorities retrieved successfully",
                    Data = priorities
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve priorities",
                    Error = ex.Message
                };
            }
        }
    }
}
