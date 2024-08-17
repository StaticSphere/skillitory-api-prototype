using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.DataServices.Audit.Interfaces;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.DataStore.Common.DataServices.Audit;

[ExcludeFromCodeCoverage]
public class CreateAuditRecordDataService : ICreateAuditRecordDataService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ISkillitoryDbContext _dbContext;

    public CreateAuditRecordDataService(ISkillitoryDbContext dbContext, IDateTimeService dateTimeService)
    {
        _dbContext = dbContext;
        _dateTimeService = dateTimeService;
    }

    public async Task ExecuteAsync(string userUniqueKey, AuditLogTypeEnum auditLogType, object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var userId = await _dbContext.Users
            .Where(u => u.UserUniqueKey == userUniqueKey)
            .Select(u => u.Id)
            .FirstAsync(cancellationToken);

        await ExecuteAsync(userId, auditLogType, metadata, cancellationToken);
    }

    public async Task ExecuteAsync(int userId, AuditLogTypeEnum auditLogType, object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            AuditLogTypeId = auditLogType,
            TimeStamp = _dateTimeService.UtcNow
        };
        _dbContext.AuditLogs.Add(auditLog);

        if (metadata is not null)
        {
            var auditLogMetadata = new AuditLogMetaData
            {
                AuditLog = auditLog,
                MetaData = JsonSerializer.Serialize(metadata)
            };
            _dbContext.AuditLogMetaDatas.Add(auditLogMetadata);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
