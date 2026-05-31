using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Statuses.command
{
    public class DeleteStatusCommand : IRequest<APIResponse>
    {
        public int Id { get; set; }
    }
}
