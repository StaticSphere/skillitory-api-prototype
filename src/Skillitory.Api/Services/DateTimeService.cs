using System.Diagnostics.CodeAnalysis;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

[ExcludeFromCodeCoverage]
public class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTime.UtcNow;
}
