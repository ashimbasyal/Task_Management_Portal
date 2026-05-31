using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.command
{
    public class DeleteSprintStatusTriggerCommand : IRequest<APIResponse>
    {
        public int Id { get; set; }
    }
}
