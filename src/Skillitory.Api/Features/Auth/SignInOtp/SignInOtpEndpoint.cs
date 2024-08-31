using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignInOtp;

public class SignInOtpEndpoint : AuthTokensEndpoint<SignInOtpCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IDateTimeService _dateTimeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditService _auditService;
    private readonly SecurityConfiguration _securityConfiguration;

    public SignInOtpEndpoint(
        UserManager<AuthUser> userManager,
        ITokenService tokenService,
        IDateTimeService dateTimeService,
        IHttpContextAccessor httpContextAccessor,
        IAuditService auditService,
        IOptions<SecurityConfiguration> securityConfiguration
        )
        : base(userManager, tokenService)
    {
        _userManager = userManager;
        _dateTimeService = dateTimeService;
        _httpContextAccessor = httpContextAccessor;
        _auditService = auditService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/sign-in-otp");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok>> ExecuteAsync(SignInOtpCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.Unauthorized();

        var otpVerified = await _userManager.VerifyTwoFactorTokenAsync(user,
            req.OtpType == OtpTypeEnum.Email
                ? TokenOptions.DefaultEmailProvider
                : TokenOptions.DefaultAuthenticatorProvider, req.Otp);

        if (!otpVerified)
            return TypedResults.Unauthorized();

        var authTokenResponse = await GenerateAuthTokensAsync(user, ct);
        var userRefreshToken = new UserRefreshToken
        {
            Token = authTokenResponse.RefreshToken,
            ExpirationDateTime = authTokenResponse.RefreshTokenExpiration,
            CreatedDateTime = _dateTimeService.UtcNow
        };
        user.RefreshTokens.Add(userRefreshToken);
        await _userManager.UpdateAsync(user);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        if (!req.UseCookies) return TypedResults.Ok(authTokenResponse);

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.AccessCookieName,
            authTokenResponse.AccessToken,
            new CookieOptions
            {
                Expires = authTokenResponse.RefreshTokenExpiration,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.RefreshCookieName,
            authTokenResponse.RefreshToken,
            new CookieOptions
            {
                Expires = authTokenResponse.RefreshTokenExpiration,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        return TypedResults.Ok();
    }
}
