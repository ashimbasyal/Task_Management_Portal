using MediatR;
using TaskManagement.Application.AuditLogs.DTOs;
using TaskManagement.Application.AuditLogs.Interfaces;

namespace TaskManagement.Application.AuditLogs.Queries;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    : IRequestHandler<GetAuditLogsQuery, PaginatedAuditLogsDto>
{
    public async Task<PaginatedAuditLogsDto> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await auditLogRepository.GetAllAsync(
            request.TableName, request.Action, request.From, request.To,
            request.Page, request.PageSize, cancellationToken);

        var totalCount = await auditLogRepository.GetTotalCountAsync(
            request.TableName, request.Action, request.From, request.To, cancellationToken);

        return new PaginatedAuditLogsDto(
            logs.Select(l => new AuditLogDto(
                l.Id, l.TableName, l.Action, l.RecordId,
                l.OldValues, l.NewValues, l.ChangedBy, l.ChangedAt
            )).ToList(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
