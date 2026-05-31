using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Priorities.command
{
    public class UpdatePriorityCommand:IRequest<APIResponse>
    {
        public int Id { get; init; }
        public string? Name { get; init; } 
    }
}
