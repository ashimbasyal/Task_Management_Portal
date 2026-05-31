using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class BulkUploadBacklogTaskCommand:IRequest<APIResponse>
    {
        public IFormFile File { get; set; } = default!;
    }
}
