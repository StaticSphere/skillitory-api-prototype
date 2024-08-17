using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Middleware;

[ExcludeFromCodeCoverage]
public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILoggerService<ExceptionLoggingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown in Skillitory: {Message}", ex.Message);

            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            response.StatusCode = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new
            {
                message = "An error was encountered in Skillitory and has been logged."
            });
            await response.WriteAsync(result);
        }
    }
}
