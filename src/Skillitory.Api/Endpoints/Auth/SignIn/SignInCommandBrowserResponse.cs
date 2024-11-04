namespace Skillitory.Api.Endpoints.Auth.SignIn;

public record SignInCommandBrowserResponse
{
    public string UserUniqueKey { get; init; } = "";
    public string AccessToken { get; init; } = "";
    public DateTimeOffset AccessTokenExpiration { get; init; }
}
