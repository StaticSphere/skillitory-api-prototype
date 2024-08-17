namespace Skillitory.Api.Features.Auth.Common;

public record AuthTokensResponse
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public DateTimeOffset RefreshTokenExpiration { get; init; }
}
