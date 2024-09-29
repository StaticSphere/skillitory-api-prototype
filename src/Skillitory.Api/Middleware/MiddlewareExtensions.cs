using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Middleware;

[ExcludeFromCodeCoverage]
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionLoggingMiddleware>();
    }

    public static IApplicationBuilder UseAuthJtiValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthJtiValidationMiddleware>();
    }

    public static IApplicationBuilder UseXmlHttpRequest(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<XmlHttpRequestMiddleware>();
    }
}
