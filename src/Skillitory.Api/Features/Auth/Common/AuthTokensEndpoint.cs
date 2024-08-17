using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.Common;

public abstract class AuthTokensEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> where TRequest : notnull
{
    private readonly UserManager<SkillitoryUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly SecurityConfiguration _securityConfiguration;

    protected AuthTokensEndpoint(
        UserManager<SkillitoryUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        SecurityConfiguration securityConfiguration)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _securityConfiguration = securityConfiguration;
    }

    protected async Task<AuthTokensResponse?> GenerateAuthTokensAsync(SkillitoryUser user, bool useCookie, CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Sub, user.UserUniqueKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokens = _tokenService.GenerateAuthTokens(claims);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.RefreshTokenExpiration;
        await _userManager.UpdateAsync(user);

        var response = new AuthTokensResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration
        };

        if (!useCookie) return response;

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.AccessCookieName,
            response.AccessToken,
            new CookieOptions
            {
                Expires = tokens.AccessTokenExpiration,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.RefreshCookieName,
            response.RefreshToken,
            new CookieOptions
            {
                Expires = response.RefreshTokenExpiration,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        return null;
    }
}
