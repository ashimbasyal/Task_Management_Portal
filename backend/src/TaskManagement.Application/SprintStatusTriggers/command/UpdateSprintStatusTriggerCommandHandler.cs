using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class UpdateSprintStatusTriggerCommandHandler : IRequestHandler<UpdateSprintStatusTriggerCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateSprintStatusTriggerCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateSprintStatusTriggerCommand request, CancellationToken cancellationToken)
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

                trigger.Name = request.Name;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Sprint status trigger updated successfully",
                    Data = new
                    {
                        trigger.Id,
                        trigger.Name
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update sprint status trigger",
                    Error = ex.Message
                };
            }
        }
    }
}
