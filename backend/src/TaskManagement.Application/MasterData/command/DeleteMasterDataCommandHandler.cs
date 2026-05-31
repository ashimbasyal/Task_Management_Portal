using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.MasterData.command
{
    public class DeleteMasterDataCommandHandler : IRequestHandler<DeleteMasterDataCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteMasterDataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteMasterDataCommand request, CancellationToken cancellationToken)
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

                entry.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Master data deactivated successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to deactivate master data",
                    Error = ex.Message
                };
            }
        }
    }
}
