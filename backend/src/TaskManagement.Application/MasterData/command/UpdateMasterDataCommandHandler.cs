using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.MasterData.command
{
    public class UpdateMasterDataCommandHandler : IRequestHandler<UpdateMasterDataCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateMasterDataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateMasterDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entry = await _context.MasterData
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entry == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Master data not found"
                    };
                }

                if (request.Value != null)
                    entry.Value = request.Value;

                entry.DisplayOrder = request.DisplayOrder;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Master data updated successfully",
                    Data = entry
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update master data",
                    Error = ex.Message
                };
            }
        }
    }
}
