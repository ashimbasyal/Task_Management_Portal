using MediatR;
using TaskManagement.Application.AuditLogs.DTOs;

namespace TaskManagement.Application.AuditLogs.Queries;

public record GetAuditLogsQuery(
    string? TableName = null,
    string? Action = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<PaginatedAuditLogsDto>;
