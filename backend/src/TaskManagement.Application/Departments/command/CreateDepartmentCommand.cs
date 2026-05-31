using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.command
{
    public class CreateDepartmentCommand : IRequest<APIResponse>
    {
        public string? Name { get; set; }
    }
}
