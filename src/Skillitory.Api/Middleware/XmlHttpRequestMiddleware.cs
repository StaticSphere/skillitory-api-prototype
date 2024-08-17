using System.Diagnostics.CodeAnalysis;
using Microsoft.Net.Http.Headers;

namespace Skillitory.Api.Middleware;

[ExcludeFromCodeCoverage]
public class XmlHttpRequestMiddleware
{
    private readonly RequestDelegate _next;

    public XmlHttpRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.Headers[HeaderNames.XRequestedWith] = "XMLHttpRequest";

        await _next(context);
    }
}
