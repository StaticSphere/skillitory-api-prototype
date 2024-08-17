using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Common.Enumerations;

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
    public OtpTypeEnum? OtpTypeId { get; set; }

    public OtpType? OtpType { get; set; }

    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
    // public Organization? Organization { get; set; } = null!;
    // public Department? Department { get; set; } = null!;
    public SkillitoryUser? Supervisor { get; set; } = null!;
    //public StoredFile? AvatarStoredFile { get; set; } = null!;
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
