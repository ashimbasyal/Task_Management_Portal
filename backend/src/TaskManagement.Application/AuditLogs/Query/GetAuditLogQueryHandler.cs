using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.AuditLogs.DTOs;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.AuditLogs.Query
{
    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;

        public GetAuditLogQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            var retObj = new APIResponse();
            try
            {
                var auditLogs = await _context.AuditLogs
                    .OrderByDescending(x => x.ChangedAt)
                    .Select(x => new AuditLogDto
                    {
                        Id = x.Id,
                        TableName = x.TableName,
                        Action = x.Action,
                        RecordId = x.RecordId,
                        OldValues = x.OldValues,
                        NewValues = x.NewValues,
                        ChangedBy = x.ChangedBy,
                        ChangedAt = x.ChangedAt
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Audit logs fetched successfully.",
                    Data = auditLogs,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to fetch audit logs.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
