using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.BacklogTasks.DTOs
{
    public class BacklogTaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } 
        public string? Description { get; set; }
        public string? RequestedBy { get; set; } 
        public string? GitLabLink { get; set; }
        public string? Remarks { get; set; }

        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? DepartmentId { get; set; }

        public bool IsMovedToSprint { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
