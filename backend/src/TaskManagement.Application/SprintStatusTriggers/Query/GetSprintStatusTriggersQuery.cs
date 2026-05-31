using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintStatusTriggers.Query
{
    public class GetSprintStatusTriggersQuery : IRequest<APIResponse>
    {
    }
}
