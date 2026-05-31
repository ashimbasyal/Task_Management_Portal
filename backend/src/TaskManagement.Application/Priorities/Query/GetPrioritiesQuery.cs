using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Priorities.Query
{
    public class GetPrioritiesQuery:IRequest<APIResponse>
    {
    }
}
