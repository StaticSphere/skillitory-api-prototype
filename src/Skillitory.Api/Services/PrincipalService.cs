using System.Globalization;
using System.Security.Claims;
using Skillitory.Api.DataStore;
using Skillitory.Api.Exceptions;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

public class PrincipalService : IPrincipalService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PrincipalService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? CurrentClaimsPrincipal => _httpContextAccessor.HttpContext?.User;

    public int UserId =>
        CurrentClaimsPrincipal is null
            ? throw new MissingClaimsPrincipalException()
            : int.Parse(
                CurrentClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                throw new MissingClaimsPrincipalException(),
                CultureInfo.CurrentCulture);

    public bool IsAuthenticated => CurrentClaimsPrincipal?.Identity?.IsAuthenticated ?? false;

    public Guid UserUniqueKey =>
        CurrentClaimsPrincipal is null
            ? throw new MissingClaimsPrincipalException()
            : Guid.Parse(
                CurrentClaimsPrincipal.FindFirst("UniqueKey")?.Value ?? throw new MissingClaimsPrincipalException());

    public Guid? OrganizationUniqueKey
    {
        get
        {
            if (CurrentClaimsPrincipal is null)
                throw new MissingClaimsPrincipalException();

            var organizationUniqueKeyClaim = CurrentClaimsPrincipal.FindFirst("OrganizationUniqueKey");
            return organizationUniqueKeyClaim is not null
                ? Guid.Parse(organizationUniqueKeyClaim.Value)
                : null;
        }
    }

    public bool IsSkillitoryAdministrator => IsUserInAnyRole(DataStoreConstants.SkillitoryAdministratorRoleName);

    public bool IsSkillitoryViewer => IsUserInAnyRole(DataStoreConstants.SkillitoryViewerRoleName);

    public bool IsOrganizationAdministrator => IsUserInAnyRole(DataStoreConstants.OrganizationAdministratorRoleName);

    public bool IsOrganizationViewer => IsUserInAnyRole(DataStoreConstants.OrganizationViewerRoleName);

    public bool IsUserInAnyRole(params string[] roles)
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        return claimsPrincipal is not null && roles.Any(r => claimsPrincipal.IsInRole(r));
    }
}
