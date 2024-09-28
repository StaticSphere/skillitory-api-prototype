using Skillitory.Api.Models;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public record SignInOtpCommandResponse
{
    public string? AccessToken { get; init; } = "";
    public string? RefreshToken { get; init; } = "";
    public DateTimeOffset? AccessTokenExpiration { get; init; }
    public DateTimeOffset? RefreshTokenExpiration { get; init; }

    public static explicit operator SignInOtpCommandResponse(TokenData src) => new()
    {
        AccessToken = src.AccessToken,
        RefreshToken = src.RefreshToken,
        AccessTokenExpiration = src.AccessTokenExpiration,
        RefreshTokenExpiration = src.RefreshTokenExpiration
    };
}
