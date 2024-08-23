using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record SecurityConfiguration
{
    public int TrialPeriodDays { get; init; }
    public PasswordConfiguration Password { get; init; } = new();
    public LockoutConfiguration Lockout { get; init; } = new();
    public JwtConfiguration Jwt { get; init; } = new();
}
