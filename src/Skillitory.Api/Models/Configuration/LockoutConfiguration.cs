using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record LockoutConfiguration
{
    public int TimeSpanMinutes { get; init; }
    public int MaxFailedAttempts { get; init; }
}
