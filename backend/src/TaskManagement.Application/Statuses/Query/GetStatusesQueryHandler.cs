using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.Statuses.DTOs;

namespace TaskManagement.Application.Statuses.Query
{
    public class GetStatusesQueryHandler : IRequestHandler<GetStatusesQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetStatusesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetStatusesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var statuses = await _context.Statuses
                .Where(x => x.IsActive)
                .Select(x => new StatusDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Statuses retrieved successfully",
                    Data = statuses
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}
