using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record JwtConfiguration
{
    public string ValidAudience { get; init; } = "";
    public string ValidIssuer { get; init; } = "";
    public string Secret { get; init; } = "";
    public int TokenValidityMinutes { get; init; }
    public int RefreshTokenValidityDays { get; init; }
}
