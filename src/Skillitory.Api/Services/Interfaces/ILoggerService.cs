namespace Skillitory.Api.Services.Interfaces;

public interface ILoggerService<TCategoryName>
{
    void LogDebug(string messageTemplate);
    void LogDebug<T0>(string messageTemplate, T0 arg0);
    void LogDebug<T0, T1>(string messageTemplate, T0 arg0, T1 arg1);
    void LogDebug<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2);

    void LogInformation(string messageTemplate);
    void LogInformation<T0>(string messageTemplate, T0 arg0);
    void LogInformation<T0, T1>(string messageTemplate, T0 arg0, T1 arg1);
    void LogInformation<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2);

    void LogWarning(string messageTemplate);
    void LogWarning<T0>(string messageTemplate, T0 arg0);
    void LogWarning<T0, T1>(string messageTemplate, T0 arg0, T1 arg1);
    void LogWarning<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2);

    void LogError(string messageTemplate);
    void LogError<T0>(string messageTemplate, T0 arg0);
    void LogError<T0, T1>(string messageTemplate, T0 arg0, T1 arg1);
    void LogError<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2);
    void LogError(Exception exception, string messageTemplate);
    void LogError<T0>(Exception exception, string messageTemplate, T0 arg0);
    void LogError<T0, T1>(Exception exception, string messageTemplate, T0 arg0, T1 arg1);
    void LogError<T0, T1, T2>(Exception exception, string messageTemplate, T0 arg0, T1 arg1, T2 arg2);
}
