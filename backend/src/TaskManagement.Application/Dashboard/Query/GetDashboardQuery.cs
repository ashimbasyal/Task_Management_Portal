using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Dashboard.Query
{
    public class GetDashboardQuery:IRequest<APIResponse>
    {
        public int? SprintId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? DepartmentId { get; set; }
        public string? AssigneeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
