using Microsoft.AspNetCore.Identity;

namespace Skillitory.Api.DataStore.Entities.Auth;

public class AuthRole : IdentityRole<int>
{
    public string? Description { get; set; }
    public bool IsApplicationAdministratorRole { get; set; }
}
