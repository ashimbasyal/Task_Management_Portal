using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class CreateSprintStatusTriggerCommandHandler : IRequestHandler<CreateSprintStatusTriggerCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateSprintStatusTriggerCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateSprintStatusTriggerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var trigger = new SprintStatusTrigger
                {
                    Name = request.Name,
                    IsActive = true
                };

                _context.SprintStatuses.Add(trigger);
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Sprint status trigger created successfully",
                    Data = trigger,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create sprint status trigger",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
