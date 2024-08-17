namespace Skillitory.Api.Services.Interfaces;

public interface IPrincipalService
{
    int UserId { get; }
    bool IsAuthenticated { get; }

    Guid UserUniqueKey { get; }
    Guid? OrganizationUniqueKey { get; }

    bool IsSkillitoryAdministrator { get; }
    bool IsSkillitoryViewer { get; }
    bool IsOrganizationAdministrator { get; }
    bool IsOrganizationViewer { get; }

    bool IsUserInAnyRole(params string[] roles);
}
