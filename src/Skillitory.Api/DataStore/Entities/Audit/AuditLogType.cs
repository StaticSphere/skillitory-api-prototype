using Skillitory.Api.DataStore.Entities.Audit.Enumerations;

namespace Skillitory.Api.DataStore.Entities.Audit;

public class AuditLogType
{
    public AuditLogTypeEnum Id { get; set; }
    public string Name { get; set; } = "";
}
