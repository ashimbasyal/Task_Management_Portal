using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.command
{
    public class DeleteDepartmentCommand : IRequest<APIResponse>
    {
        public int Id { get; set; }
    }
}
