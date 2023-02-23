using MediFlow.Domain.Enums;

namespace MediFlow.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        AuditAction action,
        string entityType,
        Guid? entityId,
        string? oldValues = null,
        string? newValues = null,
        CancellationToken cancellationToken = default);
}