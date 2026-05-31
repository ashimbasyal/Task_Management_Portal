using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

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
                if (request.Type == MasterDataType.Assignee)
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Cannot update assignee from master data. Use User Management instead."
                    };

                bool found = request.Type switch
                {
                    MasterDataType.Status => await UpdateStatus(request, cancellationToken),
                    MasterDataType.Priority => await UpdatePriority(request, cancellationToken),
                    MasterDataType.SprintStatusTrigger => await UpdateSprintStatusTrigger(request, cancellationToken),
                    MasterDataType.Department => await UpdateDepartment(request, cancellationToken),
                    _ => false
                };

                if (!found)
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Master data not found"
                    };

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Master data updated successfully"
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

        private async Task<bool> UpdateStatus(UpdateMasterDataCommand request, CancellationToken ct)
        {
            var entity = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) return false;
            entity.Name = request.Value;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> UpdatePriority(UpdateMasterDataCommand request, CancellationToken ct)
        {
            var entity = await _context.Priorities.FirstOrDefaultAsync(x => x.ID == request.Id, ct);
            if (entity == null) return false;
            entity.Name = request.Value;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> UpdateSprintStatusTrigger(UpdateMasterDataCommand request, CancellationToken ct)
        {
            var entity = await _context.SprintStatuses.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) return false;
            entity.Name = request.Value;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> UpdateDepartment(UpdateMasterDataCommand request, CancellationToken ct)
        {
            var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) return false;
            entity.Name = request.Value ?? entity.Name;
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
