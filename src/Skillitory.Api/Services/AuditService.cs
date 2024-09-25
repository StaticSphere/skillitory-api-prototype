using Skillitory.Api.DataStore.Common.DataServices.Audit.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

public class AuditService : IAuditService
{
    private readonly ICreateAuditRecordDataService _createAuditRecordDataService;

    public AuditService(ICreateAuditRecordDataService createAuditRecordDataService)
    {
        _createAuditRecordDataService = createAuditRecordDataService;
    }

    public async Task AuditUserActionAsync(int userId, AuditLogTypeEnum auditLogType,
        CancellationToken cancellationToken = default)
    {
        await _createAuditRecordDataService.ExecuteAsync(userId, auditLogType, null, cancellationToken);
    }

    public async Task AuditUserActionAsync(int userId, AuditLogTypeEnum auditLogType, object metadata,
        CancellationToken cancellationToken = default)
    {
        await _createAuditRecordDataService.ExecuteAsync(userId, auditLogType, metadata, cancellationToken);
    }

    public async Task AuditUserActionAsync(string userUniqueKey, AuditLogTypeEnum auditLogType,
        CancellationToken cancellationToken = default)
    {
        await _createAuditRecordDataService.ExecuteAsync(userUniqueKey, auditLogType, null, cancellationToken);
    }

    public async Task AuditUserActionAsync(string userUniqueKey, AuditLogTypeEnum auditLogType, object metadata,
        CancellationToken cancellationToken = default)
    {
        await _createAuditRecordDataService.ExecuteAsync(userUniqueKey, auditLogType, metadata, cancellationToken);
    }
}
