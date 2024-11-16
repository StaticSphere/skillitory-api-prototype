using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignOut;

public class SignOutEndpoint : Endpoint<SignOutCommand, NoContent>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ICookieService _cookieService;
    private readonly SecurityConfiguration _securityConfiguration;

    public SignOutEndpoint(
        IHttpContextAccessor httpContextAccessor,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        ICookieService cookieService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _cookieService = cookieService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/sign-out");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(SignOutCommand req, CancellationToken ct)
    {
        var refreshToken = req.RefreshToken;
        if (req.IsBrowser)
        {
            refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies[_securityConfiguration.RefreshCookieName] ?? string.Empty;
        }

        await _userRefreshTokenDataService.DeleteUserRefreshTokenAsync(refreshToken, ct);

        if (req.IsBrowser)
        {
            _cookieService.ClearRefreshTokenCookie();
        }

        return TypedResults.NoContent();
    }
}
