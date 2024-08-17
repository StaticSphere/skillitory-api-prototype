using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignInOtp;

public class SignInOtpEndpoint : AuthTokensEndpoint<SignInOtpCommand, Results<UnauthorizedHttpResult, Ok, Ok<AuthTokensResponse>>>
{
    private readonly UserManager<SkillitoryUser> _userManager;
    private readonly IAuditService _auditService;

    public SignInOtpEndpoint(
        UserManager<SkillitoryUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IAuditService auditService,
        IOptions<SecurityConfiguration> securityConfiguration)
        : base(userManager, httpContextAccessor, tokenService, securityConfiguration.Value)
    {
        _userManager = userManager;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in-otp");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok, Ok<AuthTokensResponse>>> ExecuteAsync(SignInOtpCommand req, CancellationToken ct)
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

        var authTokenResponse = await GenerateAuthTokensAsync(user, req.UseCookies, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        return authTokenResponse is null
            ? TypedResults.Ok()
            : TypedResults.Ok(authTokenResponse);
    }
}
