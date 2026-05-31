using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class UpdateSprintStatusTriggerCommand : IRequest<APIResponse>
    {
        public int Id { get; init; }
        public string? Name { get; init; }
    }
}
