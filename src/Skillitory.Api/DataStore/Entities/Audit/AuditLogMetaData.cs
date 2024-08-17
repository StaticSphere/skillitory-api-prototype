namespace Skillitory.Api.DataStore.Entities.Audit;

public class AuditLogMetaData
{
    public int Id { get; set; }
    public int AuditLogId { get; set; }
    public string MetaData { get; set; } = "";

    public AuditLog AuditLog { get; set; } = null!;
}
