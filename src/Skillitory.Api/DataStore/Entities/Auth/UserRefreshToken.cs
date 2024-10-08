namespace Skillitory.Api.DataStore.Entities.Auth;

public class UserRefreshToken
{
    public string UniqueKey { get; set; } = "";
    public string Jti { get; set; } = "";
    public int UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset ExpirationDateTime { get; set; }

    public AuthUser User { get; set; } = null!;
}
