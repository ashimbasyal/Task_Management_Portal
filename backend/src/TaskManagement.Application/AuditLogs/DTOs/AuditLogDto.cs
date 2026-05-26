using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.AuditLogs.DTOs
{
    public class AuditLogDto
    {
        public long Id { get; set; }

        public string? TableName { get; set; } 

        public string? Action { get; set; } 

        public int? RecordId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}
