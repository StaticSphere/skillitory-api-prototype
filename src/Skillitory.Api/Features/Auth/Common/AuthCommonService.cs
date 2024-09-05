using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.Common;

public class AuthCommonService : IAuthCommonService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SecurityConfiguration _securityConfiguration;

    public AuthCommonService(
        UserManager<AuthUser> userManager,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _securityConfiguration = securityConfiguration.Value;
    }

    public async Task<AuthTokensResponse> GenerateAuthTokensAsync(AuthUser user, CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.UserUniqueKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokens = _tokenService.GenerateAuthTokens(claims);
        return new AuthTokensResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration
        };
    }

    public void SetAuthCookies(AuthTokensResponse authTokensResponse, bool? persistedSignIn = null)
    {
        persistedSignIn ??=
            _httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(_securityConfiguration.AccessCookieName);

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.AccessCookieName,
            authTokensResponse.AccessToken,
            new CookieOptions
            {
                Expires = persistedSignIn.Value ? authTokensResponse.RefreshTokenExpiration : null,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.RefreshCookieName,
            authTokensResponse.RefreshToken,
            new CookieOptions
            {
                Expires = persistedSignIn.Value ? authTokensResponse.RefreshTokenExpiration : null,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });
    }
}
