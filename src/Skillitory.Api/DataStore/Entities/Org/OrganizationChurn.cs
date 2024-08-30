using Skillitory.Api.DataStore.Common;

namespace Skillitory.Api.DataStore.Entities.Org;

public class OrganizationChurn : IAuditableEntity
{
    public int OrganizationChurnId { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationChurnCategoryId { get; set; }
    public string Details { get; set; } = "";
    public bool IsChurned { get; set; }

    public Organization Organization { get; set; } = null!;
    public OrganizationChurnCategory OrganizationChurnCategory { get; set; } = null!;
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
