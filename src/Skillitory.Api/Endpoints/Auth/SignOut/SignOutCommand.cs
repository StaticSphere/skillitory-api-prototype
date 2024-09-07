namespace Skillitory.Api.Endpoints.Auth.SignOut;

public record SignOutCommand
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public bool UseCookies { get; set; }
}
