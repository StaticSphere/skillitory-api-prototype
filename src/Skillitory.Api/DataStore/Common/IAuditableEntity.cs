namespace Skillitory.Api.DataStore.Common;

public interface IAuditableEntity
{
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
