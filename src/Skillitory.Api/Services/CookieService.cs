using Microsoft.Extensions.Options;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SecurityConfiguration _securityConfiguration;
    private readonly bool _isDevelopment;

    public CookieService(IHttpContextAccessor httpContextAccessor,
        IOptions<SecurityConfiguration> securityConfiguration,
        IHostEnvironment hostEnvironment)
    {
        _httpContextAccessor = httpContextAccessor;
        _securityConfiguration = securityConfiguration.Value;
        _isDevelopment = hostEnvironment.IsDevelopment();
    }

    public void SetRefreshTokenCookie(string refreshToken, DateTimeOffset refreshTokenExpiration)
    {
        var persistedSignInCookieValue =
            _httpContextAccessor.HttpContext?.Request.Cookies[_securityConfiguration.PersistSignInCookieName];
        var persistedSignIn = !string.IsNullOrWhiteSpace(persistedSignInCookieValue) &&
                              persistedSignInCookieValue == "true";

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.RefreshCookieName,
            refreshToken,
            new CookieOptions
            {
                Expires = persistedSignIn ? refreshTokenExpiration : null,
                Domain = _isDevelopment ? null : _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = !_isDevelopment,
                SameSite = SameSiteMode.Lax,
            });
    }
}
