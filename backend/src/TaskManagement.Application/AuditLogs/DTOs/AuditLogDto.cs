namespace TaskManagement.Application.AuditLogs.DTOs;

public record AuditLogDto(
    long Id,
    string TableName,
    string Action,
    int? RecordId,
    string? OldValues,
    string? NewValues,
    string? ChangedBy,
    DateTime ChangedAt
);

public record PaginatedAuditLogsDto(
    IReadOnlyList<AuditLogDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
