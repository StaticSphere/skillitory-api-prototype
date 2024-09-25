using Skillitory.Api.DataStore.Entities.Audit.Enumerations;

namespace Skillitory.Api.Services.Interfaces;

public interface IAuditService
{
    Task AuditUserActionAsync(int userId, AuditLogTypeEnum auditLogType, CancellationToken cancellationToken = default);

    Task AuditUserActionAsync(int userId, AuditLogTypeEnum auditLogType, object metadata,
        CancellationToken cancellationToken = default);

    Task AuditUserActionAsync(string userUniqueKey, AuditLogTypeEnum auditLogType,
        CancellationToken cancellationToken = default);

    Task AuditUserActionAsync(string userUniqueKey, AuditLogTypeEnum auditLogType, object metadata,
        CancellationToken cancellationToken = default);
}
