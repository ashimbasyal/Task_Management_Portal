using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.MasterData.Query
{
    public class GetMasterDataByTypeQueryHandler : IRequestHandler<GetMasterDataByTypeQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetMasterDataByTypeQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetMasterDataByTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                object entries = request.Type switch
                {
                    MasterDataType.Status => await _context.Statuses
                        .Where(x => x.IsActive)
                        .Select(x => new
                        {
                            x.Id,
                            Type = (int)MasterDataType.Status,
                            Value = x.Name ?? string.Empty,
                            DisplayOrder = x.Id
                        })
                        .ToListAsync(cancellationToken),

                    MasterDataType.Priority => await _context.Priorities
                        .Where(x => x.IsActive)
                        .Select(x => new
                        {
                            x.ID,
                            Type = (int)MasterDataType.Priority,
                            Value = x.Name ?? string.Empty,
                            DisplayOrder = x.ID
                        })
                        .ToListAsync(cancellationToken),

                    MasterDataType.SprintStatusTrigger => await _context.SprintStatuses
                      
                        .Select(x => new
                        {
                            x.Id,
                            Type = (int)MasterDataType.SprintStatusTrigger,
                            Value = x.Name ?? string.Empty,
                            DisplayOrder = x.Id
                        })
                        .ToListAsync(cancellationToken),

                    MasterDataType.Assignee => await _context.Users
                        .Select(x => new
                        {
                             x.Id,
                            Type = (int)MasterDataType.Assignee,
                            Value = x.FullName,
                            DisplayOrder = 0
                        })
                        .ToListAsync(cancellationToken),

                    MasterDataType.Department => await _context.Departments
                        .Where(x => x.IsActive)
                        .Select(x => new
                        {
                             x.Id,
                            Type = (int)MasterDataType.Department,
                            Value = x.Name,
                            DisplayOrder = x.Id
                        })
                        .ToListAsync(cancellationToken),

                    _ => Enumerable.Empty<object>()
                };

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Master data retrieved successfully",
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve master data",
                    Error = ex.Message
                };
            }
        }
    }
}
