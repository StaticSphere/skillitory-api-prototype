using System.Net;
using FastEndpoints;
using Skillitory.Api.Middleware;

namespace Skillitory.Api.Bootstrap;

public static class ApiRuntimeBootstrap
{
    public static WebApplication UseApiRuntime(this WebApplication app, string corsPolicy)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(corsPolicy);

        app.UseExceptionLogging();
        app.UseXmlHttpRequest();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAuthJtiValidation();

        app.MapHealthChecks("/health");

        app.UseFastEndpoints(options =>
        {
            options.Errors.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            options.Versioning.PrependToRoute = true;
            options.Versioning.Prefix = "v";
            options.Versioning.DefaultVersion = 1;
        });

        return app;
    }
}
