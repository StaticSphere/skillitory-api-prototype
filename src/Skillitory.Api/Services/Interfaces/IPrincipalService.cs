namespace Skillitory.Api.Services.Interfaces;

public interface IPrincipalService
{
    string UserUniqueKey { get; }
    bool IsAuthenticated { get; }

    string? OrganizationUniqueKey { get; }

    bool IsSkillitoryAdministrator { get; }
    bool IsSkillitoryViewer { get; }
    bool IsOrganizationAdministrator { get; }
    bool IsOrganizationViewer { get; }

    bool IsUserInAnyRole(params string[] roles);
}
