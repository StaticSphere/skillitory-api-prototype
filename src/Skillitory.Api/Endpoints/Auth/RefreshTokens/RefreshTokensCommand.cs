namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public record RefreshTokensCommand
{
    public string? RefreshToken { get; init; }
    public bool IsBrowser { get; init; }
}
