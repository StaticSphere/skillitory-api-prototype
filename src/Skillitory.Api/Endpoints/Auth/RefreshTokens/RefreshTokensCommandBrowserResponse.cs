using Skillitory.Api.Models;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public record RefreshTokensCommandBrowserResponse
{
    public string AccessToken { get; init; } = "";
    public DateTimeOffset AccessTokenExpiration { get; init; }

    public static explicit operator RefreshTokensCommandBrowserResponse(TokenData src) => new()
    {
        AccessToken = src.AccessToken,
        AccessTokenExpiration = src.AccessTokenExpiration
    };
}
