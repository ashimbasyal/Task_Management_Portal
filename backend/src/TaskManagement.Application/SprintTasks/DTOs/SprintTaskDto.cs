using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.SprintTasks.DTOs
{
    public class SprintTaskDto
    {
        public int Id { get; set; }

        public int BacklogTaskId { get; set; }

        public int? BacklogTaskSN { get; set; }

        public string? BacklogTaskTitle { get; set; }

        public string? SprintName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Remarks { get; set; }

        public string? AssigneeId { get; set; }

        public string? AssigneeName { get; set; }

        public int? StatusId { get; set; }

        public string? StatusName { get; set; }

        public string? PriorityName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
