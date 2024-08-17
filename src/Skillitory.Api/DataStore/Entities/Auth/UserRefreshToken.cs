namespace Skillitory.Api.DataStore.Entities.Auth;

public class UserRefreshToken
{
    public int UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset ExpirationDateTime { get; set; }

    public SkillitoryUser User { get; set; } = null!;
}
