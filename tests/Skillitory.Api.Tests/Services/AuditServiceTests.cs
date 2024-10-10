using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Audit.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.Services;

namespace Skillitory.Api.Tests.Services;

public class AuditServiceTests
{
    private readonly AuditService _auditService;
    private readonly ICreateAuditRecordDataService _createAuditRecordDataService;

    public AuditServiceTests()
    {
        _createAuditRecordDataService = Substitute.For<ICreateAuditRecordDataService>();
        _auditService = new AuditService(_createAuditRecordDataService);
    }

    [Fact]
    public async Task ServiceAuditsWithUserIdAndNoMetadata()
    {
        await _auditService.AuditUserActionAsync(1, AuditLogTypeEnum.NewUserEmailVerified);

        await _createAuditRecordDataService.Received(1).ExecuteAsync(1, AuditLogTypeEnum.NewUserEmailVerified);
    }

    [Fact]
    public async Task ServiceAuditsWithUserIdAndMetadata()
    {
        await _auditService.AuditUserActionAsync(1, AuditLogTypeEnum.NewUserEmailVerified, "Test");

        await _createAuditRecordDataService.Received(1).ExecuteAsync(1, AuditLogTypeEnum.NewUserEmailVerified, "Test");
    }

    [Fact]
    public async Task ServiceAuditsWithUniqueKeyAndNoMetadata()
    {
        var uniqueKey = Guid.NewGuid().ToString();
        await _auditService.AuditUserActionAsync(uniqueKey, AuditLogTypeEnum.NewUserEmailVerified);

        await _createAuditRecordDataService.Received(1).ExecuteAsync(uniqueKey, AuditLogTypeEnum.NewUserEmailVerified);
    }

    [Fact]
    public async Task ServiceAuditsWithUniqueKeyAndMetadata()
    {
        var uniqueKey = Guid.NewGuid().ToString();
        await _auditService.AuditUserActionAsync(uniqueKey, AuditLogTypeEnum.NewUserEmailVerified, "Test");

        await _createAuditRecordDataService.Received(1)
            .ExecuteAsync(uniqueKey, AuditLogTypeEnum.NewUserEmailVerified, "Test");
    }
}
