using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dashboard.DTOs
{
    public class SprintTaskCountDto
    {
        public string SprintName { get; set; }
        public int TaskCount { get; set; }
    }
}
