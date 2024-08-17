using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common;

namespace Skillitory.Api.DataStore.Entities.Auth;

public class SkillitoryUser : IdentityUser<int>, IAuditableEntity
{
    public string UserUniqueKey { get; set; } = "";
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
    public bool IsSignInAllowed { get; set; }
    public bool IsSystemUser { get; set; }
    public DateTimeOffset? TerminatedOn { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }

    // public Organization? Organization { get; set; }
    // public Department? Department { get; set; }
    public SkillitoryUser? Supervisor { get; set; }
    //public StoredFile? AvatarStoredFile { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
}
