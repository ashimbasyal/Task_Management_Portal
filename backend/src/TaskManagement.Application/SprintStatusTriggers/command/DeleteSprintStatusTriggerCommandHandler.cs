using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class DeleteSprintStatusTriggerCommandHandler : IRequestHandler<DeleteSprintStatusTriggerCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteSprintStatusTriggerCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteSprintStatusTriggerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var trigger = await _context.SprintStatuses
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (trigger == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Sprint status trigger not found"
                    };
                }

                trigger.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint status trigger deactivated successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to deactivate sprint status trigger",
                    Error = ex.Message
                };
            }
        }
    }
}
