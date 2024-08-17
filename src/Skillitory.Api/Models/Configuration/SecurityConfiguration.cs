using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record SecurityConfiguration
{
    public int SignInCookieHours { get; init; }
    public int TrialPeriodDays { get; init; }
    public string AccessCookieName { get; init; } = "";
    public string RefreshCookieName { get; init; } = "";
    public string AuthCookieDomain { get; init; } = "";
    public PasswordConfiguration Password { get; init; } = new();
    public LockoutConfiguration Lockout { get; init; } = new();
    public JwtConfiguration Jwt { get; init; } = new();
}
