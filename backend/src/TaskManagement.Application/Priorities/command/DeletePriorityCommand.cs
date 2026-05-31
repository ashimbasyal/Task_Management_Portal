using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Priorities.command
{
    public class DeletePriorityCommand:IRequest<APIResponse>
    {
        public int ID {  get; set; }
    }
}
