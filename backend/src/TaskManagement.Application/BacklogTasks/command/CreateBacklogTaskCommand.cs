using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class CreateBacklogTaskCommand: IRequest<APIResponse>
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? RequestedBy { get; set; }

        public string? GitLabLink { get; set; }

        public string? Remarks { get; set; }

        public int? PriorityId { get; set; }

        public int? StatusId { get; set; }

        public int? DepartmentId { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
