namespace Skillitory.Api.DataStore.Entities.Auth;

public class PasswordHistory
{
    public int UserId { get; set; }
    public string PasswordHash { get; set; } = "";
    public DateTimeOffset CreatedDateTime { get; set; }

    public AuthUser User { get; set; } = null!;
}
