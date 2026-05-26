using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.AuditLogs.Command
{
    public class UpdateAuditLogCommandHandler : IRequestHandler<UpdateAuditLogCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateAuditLogCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateAuditLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var auditLog = await _context.AuditLogs.FindAsync(new object[] { request.Id }, cancellationToken);

                if (auditLog == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Audit log not found.",
                        Data = null,
                        Error = null
                    };
                }

                // Update fields
                auditLog.TableName = request.TableName;
                auditLog.Action = request.Action;
                auditLog.RecordId = request.RecordId;
                auditLog.OldValues = request.OldValues;
                auditLog.NewValues = request.NewValues;
                auditLog.ChangedBy = request.ChangedBy;
                auditLog.ChangedAt = DateTime.UtcNow;

                _context.AuditLogs.Update(auditLog);
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Audit log updated successfully.",
                    Data = auditLog,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update audit log.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}

