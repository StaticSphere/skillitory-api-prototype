using Skillitory.Api.Models;

namespace Skillitory.Api.Endpoints.Auth.SignIn;

public record SignInCommandResponse
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public DateTimeOffset AccessTokenExpiration { get; init; }
    public DateTimeOffset RefreshTokenExpiration { get; init; }

    public static explicit operator SignInCommandResponse(TokenData src) => new()
    {
        AccessToken = src.AccessToken,
        RefreshToken = src.RefreshToken,
        AccessTokenExpiration = src.AccessTokenExpiration,
        RefreshTokenExpiration = src.RefreshTokenExpiration
    };
}
