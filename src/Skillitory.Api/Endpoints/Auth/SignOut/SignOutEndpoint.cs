using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignOut;

public class SignOutEndpoint : Endpoint<SignOutCommand, NoContent>
{
    private readonly ITokenService _tokenService;
    private readonly ISignOutDataService _signOutDataService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SecurityConfiguration _securityConfiguration;

    public SignOutEndpoint(
        ITokenService tokenService,
        ISignOutDataService signOutDataService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _tokenService = tokenService;
        _signOutDataService = signOutDataService;
        _httpContextAccessor = httpContextAccessor;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/sign-out");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(SignOutCommand req, CancellationToken ct)
    {
        var claimsPrincipal = _tokenService.GetClaimsPrincipalFromAccessToken(req.AccessToken);

        var userUniqueKey = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userUniqueKey)) return TypedResults.NoContent();

        await _signOutDataService.DeleteUserRefreshTokenAsync(userUniqueKey, req.RefreshToken, ct);

        if (!req.UseCookies) return TypedResults.NoContent();

        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(_securityConfiguration.AccessCookieName);
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(_securityConfiguration.RefreshCookieName);

        return TypedResults.NoContent();
    }
}
