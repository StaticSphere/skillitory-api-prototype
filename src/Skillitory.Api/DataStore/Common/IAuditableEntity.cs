namespace Skillitory.Api.DataStore.Common;

public interface IAuditableEntity
{
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
}
