using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Entities.Audit;

public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AuditLogTypeEnum AuditLogTypeId { get; set; }
    public DateTimeOffset TimeStamp { get; set; }

    public AuthUser User { get; set; } = null!;
    public AuditLogType AuditLogType { get; set; } = null!;
}
