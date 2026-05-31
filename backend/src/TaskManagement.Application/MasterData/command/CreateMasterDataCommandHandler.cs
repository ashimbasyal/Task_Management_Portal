using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.MasterData.command
{
    public class CreateMasterDataCommandHandler : IRequestHandler<CreateMasterDataCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateMasterDataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateMasterDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entry = new Domain.Entities.MasterData
                {
                    Type = request.Type,
                    Value = request.Value,
                    DisplayOrder = request.DisplayOrder,
                    IsActive = true
                };

                _context.MasterData.Add(entry);
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Master data created successfully",
                    Data = entry,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create master data",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
