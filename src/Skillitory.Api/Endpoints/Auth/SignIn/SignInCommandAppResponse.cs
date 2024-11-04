namespace Skillitory.Api.Endpoints.Auth.SignIn;

public record SignInCommandAppResponse
{
    public string UserUniqueKey { get; init; } = "";
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public DateTimeOffset AccessTokenExpiration { get; init; }
    public DateTimeOffset RefreshTokenExpiration { get; init; }
}
