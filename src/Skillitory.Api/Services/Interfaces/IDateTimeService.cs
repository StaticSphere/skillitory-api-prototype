namespace Skillitory.Api.Services.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}
