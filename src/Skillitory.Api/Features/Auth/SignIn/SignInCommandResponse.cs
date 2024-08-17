namespace Skillitory.Api.Features.Auth.SignIn;

public record SignInCommandResponse
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public DateTimeOffset RefreshTokenExpiration { get; init; }
}
