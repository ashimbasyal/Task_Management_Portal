using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintTasks.Query
{
    public class GetAllSprintTasksQuery : IRequest<APIResponse>
    {
    }
}
