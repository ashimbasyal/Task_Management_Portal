using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class CreateSprintStatusTriggerCommand : IRequest<APIResponse>
    {
        public string? Name { get; set; }
    }
}
