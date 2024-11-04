using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Auth.Enumerations;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignIn;



public class SignInEndpoint : Endpoint<SignInCommand, Results<
    UnauthorizedHttpResult,
    Ok<SignInCommandAppResponse>,
    Ok<SignInCommandBrowserResponse>,
    Ok<SignInCommandOtpResponse>
>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;
    private readonly SecurityConfiguration _securityConfiguration;

    public SignInEndpoint(
        UserManager<AuthUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        ITokenService tokenService,
        IEmailService emailService,
        IDateTimeService dateTimeService,
        IAuditService auditService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _tokenService = tokenService;
        _emailService = emailService;
        _dateTimeService = dateTimeService;
        _auditService = auditService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<
        UnauthorizedHttpResult,
        Ok<SignInCommandAppResponse>,
        Ok<SignInCommandBrowserResponse>,
        Ok<SignInCommandOtpResponse>
    >> ExecuteAsync(SignInCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOnDateTime.HasValue)
            return TypedResults.Unauthorized();

        if (!(await _userManager.CheckPasswordAsync(user, req.Password)))
            return TypedResults.Unauthorized();

        if (user.TwoFactorEnabled)
        {
            if (user.OtpTypeId == OtpTypeEnum.Email)
            {
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await _emailService.SendSignInOtpEmailAsync(user.Email!, otp, ct);
            }

            return TypedResults.Ok(new SignInCommandOtpResponse{ OtpType = OtpTypeEnum.Email, UserUniqueKey = user.UserUniqueKey });
        }

        var jti = Guid.NewGuid();
        var tokens = await _tokenService.GenerateAuthTokensAsync(user, jti, ct);

        await _userRefreshTokenDataService.SaveNewUserRefreshTokenAsync(user.Id, jti, tokens.RefreshToken, tokens.RefreshTokenExpiration, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        user.LastSignInDateTime = _dateTimeService.UtcNow;
        await _userManager.UpdateAsync(user);

        if (!req.IsBrowser)
        {
            return TypedResults.Ok(new SignInCommandAppResponse
            {
                UserUniqueKey = user.UserUniqueKey,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiration = tokens.AccessTokenExpiration,
                RefreshTokenExpiration = tokens.RefreshTokenExpiration,
            });
        }

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            _securityConfiguration.RefreshCookieName,
            tokens.RefreshToken,
            new CookieOptions
            {
                Expires = tokens.RefreshTokenExpiration,
                Domain = _securityConfiguration.AuthCookieDomain,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });

        return TypedResults.Ok(new SignInCommandBrowserResponse
        {
            UserUniqueKey = user.UserUniqueKey,
            AccessToken = tokens.AccessToken,
            AccessTokenExpiration = tokens.AccessTokenExpiration
        });
    }
}
