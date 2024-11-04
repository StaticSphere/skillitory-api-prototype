namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public record SignInOtpCommandBrowserResponse
{
    public string UserUniqueKey { get; init; } = "";
    public string AccessToken { get; init; } = "";
    public DateTimeOffset AccessTokenExpiration { get; init; }
}
