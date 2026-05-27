using MediatR;
using TaskManagement.Application.AuditLogs.DTOs;

namespace TaskManagement.Application.AuditLogs.Queries;

public record GetAuditLogsQuery : IRequest<IReadOnlyList<AuditLogDto>>;
