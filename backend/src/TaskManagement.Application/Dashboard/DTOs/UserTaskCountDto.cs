using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Dashboard.DTOs
{
    public class UserTaskCountDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TaskCount { get; set; }
    }
}
