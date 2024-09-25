using Skillitory.Api.DataStore.Entities.Audit.Enumerations;

namespace Skillitory.Api.DataStore.Common.DataServices.Audit.Interfaces;

public interface ICreateAuditRecordDataService
{
    Task ExecuteAsync(string userUniqueKey, AuditLogTypeEnum auditLogType, object? metadata = null,
        CancellationToken cancellationToken = default);

    Task ExecuteAsync(int userId, AuditLogTypeEnum auditLogType, object? metadata = null,
        CancellationToken cancellationToken = default);
}
