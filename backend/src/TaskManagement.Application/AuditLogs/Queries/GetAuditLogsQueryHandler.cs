using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.AuditLogs.DTOs;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.AuditLogs.Queries;

public sealed class GetAuditLogsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAuditLogsQuery, PaginatedAuditLogsDto>
{
    public async Task<PaginatedAuditLogsDto> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = context.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.TableName))
            query = query.Where(a => a.TableName == request.TableName);

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action);

        if (request.From.HasValue)
            query = query.Where(a => a.ChangedAt >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(a => a.ChangedAt <= request.To.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(a => a.ChangedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

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
