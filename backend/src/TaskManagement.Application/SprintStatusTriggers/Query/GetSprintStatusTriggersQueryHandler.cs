using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.Query
{
    public class GetSprintStatusTriggersQueryHandler : IRequestHandler<GetSprintStatusTriggersQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetSprintStatusTriggersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetSprintStatusTriggersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var triggers = await _context.SprintStatuses
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint status triggers retrieved successfully",
                    Data = triggers
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve sprint status triggers",
                    Error = ex.Message
                };
            }
        }
    }
}
