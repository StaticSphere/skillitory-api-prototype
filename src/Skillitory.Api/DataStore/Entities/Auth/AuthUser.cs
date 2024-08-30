using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Mbr;

namespace Skillitory.Api.DataStore.Entities.Auth;

public class AuthUser : IdentityUser<int>, IAuditableEntity
{
    public string UserUniqueKey { get; set; } = "";
    public bool IsSignInAllowed { get; set; }
    public bool IsSystemUser { get; set; }
    public DateTimeOffset? TerminatedOn { get; set; }
    public OtpTypeEnum? OtpTypeId { get; set; }
    public OtpType? OtpType { get; set; }
    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }

    public Member? Member { get; set; } = null!;
}
