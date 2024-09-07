namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public record RefreshTokensCommand
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
}
