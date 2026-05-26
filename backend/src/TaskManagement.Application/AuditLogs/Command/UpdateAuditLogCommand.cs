using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.AuditLogs.Command
{
    public class UpdateAuditLogCommand:IRequest<APIResponse>
    {
        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;

        public int? RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public string? ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
