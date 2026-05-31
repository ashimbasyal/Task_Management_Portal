using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dashboard.DTOs
{
    public class DashboardResponseDto
    {
        public List<SprintTaskCountDto> SprintWiseTaskCounts { get; set; }
        public List<StatusDistributionDto> StatusWiseDistribution { get; set; }
        public List<PriorityDistributionDto> PriorityWiseDistribution { get; set; }
        public List<DepartmentDistributionDto> DepartmentWiseDistribution { get; set; }
        public List<UserTaskCountDto> AssignedUserTaskCounts { get; set; }

        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
