using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.command
{
    public class UpdateDepartmentCommand : IRequest<APIResponse>
    {
        public int Id { get; init; }
        public string? Name { get; init; }
    }
}
