using System.Diagnostics.CodeAnalysis;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

[ExcludeFromCodeCoverage]
public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
