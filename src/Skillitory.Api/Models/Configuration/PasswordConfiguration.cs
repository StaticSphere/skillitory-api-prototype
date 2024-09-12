using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record PasswordConfiguration
{
    public int MinimumLength { get; init; }
    public int RequiredUniqueCharacters { get; init; }
    public bool RequireSymbols { get; init; }
    public int PreviousTrackedPasswordCount { get; init; }
    public int DefaultPasswordLifetimeDays { get; init; }
}
