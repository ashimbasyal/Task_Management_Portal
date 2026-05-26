using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.AuditLogs.Command
{
    public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;

        public CreateAuditLogCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    TableName = request.TableName,
                    Action = request.Action,
                    RecordId = request.RecordId,
                    OldValues = request.OldValues,
                    NewValues = request.NewValues,
                    ChangedBy = request.ChangedBy,
                    ChangedAt = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Audit log created successfully.",
                    Data = auditLog,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create audit log.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}

