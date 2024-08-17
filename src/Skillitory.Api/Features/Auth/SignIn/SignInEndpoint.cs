using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignIn;

public class SignInEndpoint : Endpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok, Ok<SignInCommandResponse>>>
{
    private readonly UserManager<SkillitoryUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;
    private readonly SecurityConfiguration _securityConfiguration;

    public SignInEndpoint(
        UserManager<SkillitoryUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IAuditService auditService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _auditService = auditService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok, Ok<SignInCommandResponse>>> ExecuteAsync(SignInCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.Unauthorized();

        if (!(await _userManager.CheckPasswordAsync(user, req.Password)))
            return TypedResults.Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Sub, user.UserUniqueKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (user.TwoFactorEnabled)
        {
        }

        var tokens = _tokenService.GenerateAuthTokens(claims);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTime = tokens.RefreshTokenExpiration;
        await _userManager.UpdateAsync(user);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        var response = new SignInCommandResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration
        };

        if (!req.UseCookie) return TypedResults.Ok(response);

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

        return TypedResults.Ok();
    }
}
