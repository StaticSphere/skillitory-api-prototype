using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore.Entities.Mbr;

public class Member : IAuditableEntity
{
    public int UserId { get; set; }
    public int? OrganizationId { get; set; }
    public int? DepartmentId { get; set; }
    public string? Title { get; set; }
    public int? SupervisorId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateOnly? BirthDate { get; set; }
    public string? Biography { get; set; }
    public string? Education { get; set; }
    public string? ExternalId { get; set; }
    public int? AvatarStoredFileId { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }

    public AuthUser User { get; set; } = null!;
    public Organization? Organization { get; set; } = null!;
    public Department? Department { get; set; } = null!;
    public Member? Supervisor { get; set; } = null!;
    //public StoredFile? AvatarStoredFile { get; set; } = null!;
}
