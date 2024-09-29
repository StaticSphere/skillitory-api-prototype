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

    public string UserUniqueKey =>
        CurrentClaimsPrincipal is null
            ? throw new MissingClaimsPrincipalException()
            : CurrentClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new MissingClaimsPrincipalException();

    public bool IsAuthenticated => CurrentClaimsPrincipal?.Identity?.IsAuthenticated ?? false;

    public string? OrganizationUniqueKey
    {
        get
        {
            if (CurrentClaimsPrincipal is null)
                throw new MissingClaimsPrincipalException();

            var organizationUniqueKeyClaim = CurrentClaimsPrincipal.FindFirst("OrganizationUniqueKey");
            return organizationUniqueKeyClaim?.Value;
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
