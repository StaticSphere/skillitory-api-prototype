namespace Skillitory.Api.Features.Auth.RefreshTokens;

public record RefreshTokensCommand
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
}
