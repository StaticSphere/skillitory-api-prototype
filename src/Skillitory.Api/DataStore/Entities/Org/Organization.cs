using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Mbr;

namespace Skillitory.Api.DataStore.Entities.Org;

public class Organization : IAuditableEntity
{
    public int OrganizationId { get; set; }
    public string OrganizationUniqueKey { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? ExternalIdName { get; set; }
    public int? LogoStoredFileId { get; set; }
    public bool IsLogoOverrideAllowed { get; set; }
    public bool IsSystemOrganization { get; set; }
    public DateOnly? TrialPeriodEndsOn { get; set; }

    public ICollection<Department> Departments { get; set; } = null!;
    public ICollection<Member> Users { get; set; } = null!;
    // public StoredFile? LogoStoredFile { get; set; }
    public ICollection<OrganizationChurn> OrganizationChurns { get; set; } = null!;
    public AuthUser CreationUser { get; set; } = null!;
    public AuthUser? UpdatingUser { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
