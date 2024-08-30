using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Entities.Org;

public class Department : IAuditableEntity
{
    public int DepartmentId { get; set; }
    public int OrganizationId { get; set; }
    public Guid DepartmentUniqueKey { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int? LogoStoredFileId { get; set; }

    public Organization Organization { get; set; } = null!;
    public AuthUser CreationUser { get; set; } = null!;
    // public StoredFile? LogoStoredFile { get; set; } = null;
    public AuthUser? UpdatingUser { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
