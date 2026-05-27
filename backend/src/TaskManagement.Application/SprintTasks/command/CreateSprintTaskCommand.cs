using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.SprintTasks.command
{
    public class CreateSprintTaskCommand: IRequest<APIResponse>
    {
        public int BacklogTaskId { get; set; }

        public string? SprintName { get; set; } 

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Remarks { get; set; }

        public int? AssigneeId { get; set; }

        public int? StatusId { get; set; }

        public string? CreatedBy { get; set; }
    }
}
