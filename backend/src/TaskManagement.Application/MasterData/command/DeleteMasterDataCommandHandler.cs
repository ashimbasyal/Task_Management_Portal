using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

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
                if (request.Type == MasterDataType.Assignee)
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Cannot delete assignee from master data. Use User Management instead."
                    };

                bool found = request.Type switch
                {
                    MasterDataType.Status => await DeactivateStatus(request.Id, cancellationToken),
                    MasterDataType.Priority => await DeactivatePriority(request.Id, cancellationToken),
                    MasterDataType.SprintStatusTrigger => await DeactivateSprintStatusTrigger(request.Id, cancellationToken),
                    MasterDataType.Department => await DeactivateDepartment(request.Id, cancellationToken),
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

        private async Task<bool> DeactivateStatus(int id, CancellationToken ct)
        {
            var entity = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> DeactivatePriority(int id, CancellationToken ct)
        {
            var entity = await _context.Priorities.FirstOrDefaultAsync(x => x.ID == id, ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> DeactivateSprintStatusTrigger(int id, CancellationToken ct)
        {
            var entity = await _context.SprintStatuses.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<bool> DeactivateDepartment(int id, CancellationToken ct)
        {
            var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
