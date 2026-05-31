using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class GetAllBacklogTaskQuery: IRequest<APIResponse>
    {
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
