using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }

        public string? Name { get; set; } 

        public ICollection<BacklogTask> BacklogTasks { get; set; }
        = new List<BacklogTask>();
    }
}
