using System.Diagnostics.CodeAnalysis;
using Serilog.Events;
using Skillitory.Api.Services.Interfaces;
using ILogger = Serilog.ILogger;
// ReSharper disable ContextualLoggerProblem
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Skillitory.Api.Services;

[ExcludeFromCodeCoverage]
public class LoggerService<TCategoryName> : ILoggerService<TCategoryName>
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger)
    {
        _logger = logger;
    }

    public void LogDebug(string messageTemplate)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger
                .ForContext<TCategoryName>()
                .Debug(messageTemplate);
        }
    }

    public void LogDebug<T0>(string messageTemplate, T0 arg0)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger
                .ForContext<TCategoryName>()
                .Debug(messageTemplate, arg0);
        }
    }

    public void LogDebug<T0, T1>(string messageTemplate, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger
                .ForContext<TCategoryName>()
                .Debug(messageTemplate, arg0, arg1);
        }
    }

    public void LogDebug<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger
                .ForContext<TCategoryName>()
                .Debug(messageTemplate, arg0, arg1, arg2);
        }
    }

    public void LogInformation(string messageTemplate)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger
                .ForContext<TCategoryName>()
                .Information(messageTemplate);
        }
    }

    public void LogInformation<T0>(string messageTemplate, T0 arg0)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger
                .ForContext<TCategoryName>()
                .Information(messageTemplate, arg0);
        }
    }

    public void LogInformation<T0, T1>(string messageTemplate, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger
                .ForContext<TCategoryName>()
                .Information(messageTemplate, arg0, arg1);
        }
    }

    public void LogInformation<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger
                .ForContext<TCategoryName>()
                .Information(messageTemplate, arg0, arg1, arg2);
        }
    }

    public void LogWarning(string messageTemplate)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger
                .ForContext<TCategoryName>()
                .Warning(messageTemplate);
        }
    }

    public void LogWarning<T0>(string messageTemplate, T0 arg0)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger
                .ForContext<TCategoryName>()
                .Warning(messageTemplate, arg0);
        }
    }

    public void LogWarning<T0, T1>(string messageTemplate, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger
                .ForContext<TCategoryName>()
                .Warning(messageTemplate, arg0, arg1);
        }
    }

    public void LogWarning<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogEventLevel.Warning))
        {
            _logger
                .ForContext<TCategoryName>()
                .Warning(messageTemplate, arg0, arg1, arg2);
        }
    }

    public void LogError(string messageTemplate)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(messageTemplate);
        }
    }

    public void LogError<T0>(string messageTemplate, T0 arg0)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(messageTemplate, arg0);
        }
    }

    public void LogError<T0, T1>(string messageTemplate, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(messageTemplate, arg0, arg1);
        }
    }

    public void LogError<T0, T1, T2>(string messageTemplate, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(messageTemplate, arg0, arg1, arg2);
        }
    }

    public void LogError(Exception exception, string messageTemplate)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(exception, messageTemplate);
        }
    }

    public void LogError<T0>(Exception exception, string messageTemplate, T0 arg0)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(exception, messageTemplate, arg0);
        }
    }

    public void LogError<T0, T1>(Exception exception, string messageTemplate, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(exception, messageTemplate, arg0, arg1);
        }
    }

    public void LogError<T0, T1, T2>(Exception exception, string messageTemplate, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogEventLevel.Error))
        {
            _logger
                .ForContext<TCategoryName>()
                .Error(exception, messageTemplate, arg0, arg1, arg2);
        }
    }
}
