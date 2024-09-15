namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public record RefreshTokensCommand
{
    public string RefreshToken { get; init; } = "";
}
