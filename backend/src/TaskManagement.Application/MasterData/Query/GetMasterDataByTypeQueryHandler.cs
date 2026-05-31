using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

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
                var entries = await _context.MasterData
                    .Where(x => x.Type == request.Type && x.IsActive)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new
                    {
                        x.Id,
                        x.Type,
                        x.Value,
                        x.DisplayOrder
                    })
                    .ToListAsync(cancellationToken);

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
