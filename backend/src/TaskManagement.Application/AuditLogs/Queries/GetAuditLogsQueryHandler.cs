using MediatR;
using TaskManagement.Application.AuditLogs.DTOs;
using TaskManagement.Application.AuditLogs.Interfaces;

namespace TaskManagement.Application.AuditLogs.Queries;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    : IRequestHandler<GetAuditLogsQuery, IReadOnlyList<AuditLogDto>>
{
    public async Task<IReadOnlyList<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await auditLogRepository.GetAllAsync(cancellationToken);
        return logs.Select(l => new AuditLogDto(
            l.Id, l.TableName, l.Action, l.RecordId,
            l.OldValues, l.NewValues, l.ChangedBy, l.ChangedAt
        )).ToList();
    }
}
