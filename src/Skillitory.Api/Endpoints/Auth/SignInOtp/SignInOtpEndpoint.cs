using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Auth.Enumerations;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public class SignInOtpEndpoint : Endpoint<SignInOtpCommand, Results<UnauthorizedHttpResult, Ok<SignInOtpCommandResponse>>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly ISignInOtpDataService _signInOtpDataService;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;

    public SignInOtpEndpoint(
        UserManager<AuthUser> userManager,
        ISignInOtpDataService signInOtpDataService,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        ITokenService tokenService,
        IDateTimeService dateTimeService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _signInOtpDataService = signInOtpDataService;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _tokenService = tokenService;
        _dateTimeService = dateTimeService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in-otp");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<SignInOtpCommandResponse>>> ExecuteAsync(SignInOtpCommand req, CancellationToken ct)
    {
        var user = await _signInOtpDataService.GetUserByUserUniqueKeyAsync(req.UserUniqueKey, ct);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOnDateTime.HasValue)
            return TypedResults.Unauthorized();

        var otpVerified = await _userManager.VerifyTwoFactorTokenAsync(user,
            req.OtpType == OtpTypeEnum.Email
                ? TokenOptions.DefaultEmailProvider
                : TokenOptions.DefaultAuthenticatorProvider, req.Otp);

        if (!otpVerified)
            return TypedResults.Unauthorized();

        var jti = Guid.NewGuid();
        var tokens = await _tokenService.GenerateAuthTokensAsync(user, jti, ct);

        await _userRefreshTokenDataService.SaveNewUserRefreshTokenAsync(user.Id, jti, tokens.RefreshToken, tokens.RefreshTokenExpiration, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        user.LastSignInDateTime = _dateTimeService.UtcNow;
        await _userManager.UpdateAsync(user);

        return TypedResults.Ok(new SignInOtpCommandResponse
        {
            UserUniqueKey = user.UserUniqueKey,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiration = tokens.AccessTokenExpiration,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration,
        });
    }
}
