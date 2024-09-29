using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Middleware;

[ExcludeFromCodeCoverage]
public class AuthJtiValidationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthJtiValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService, IUserRefreshTokenDataService userRefreshTokenDataService)
    {
        if (context.User.Identity is { IsAuthenticated: true } && context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is null)
        {
            var token = await context.GetTokenAsync("access_token");
            if (token is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var principal = tokenService.GetClaimsPrincipalFromAccessToken(token);
            var jti = principal!.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            var jtiExists = await userRefreshTokenDataService.GetRefreshTokenWithJtiExistsAsync(jti!.Value);
            if (!jtiExists)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await _next(context);
    }
}
