using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignInOtp;

public class SignInOtpEndpoint : Endpoint<SignInOtpCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IAuthCommonService _authCommonService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;

    public SignInOtpEndpoint(
        UserManager<AuthUser> userManager,
        IAuthCommonService authCommonService,
        IDateTimeService dateTimeService,
        IAuditService auditService
        )
    {
        _userManager = userManager;
        _authCommonService = authCommonService;
        _dateTimeService = dateTimeService;
        _auditService = auditService;
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

        var authTokenResponse = await _authCommonService.GenerateAuthTokensAsync(user, ct);
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

        _authCommonService.SetAuthCookies(authTokenResponse, req.PersistedSignIn);

        return TypedResults.Ok();
    }
}
