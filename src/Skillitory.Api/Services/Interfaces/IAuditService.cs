using Skillitory.Api.DataStore.Common.Enumerations;

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
