using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignIn;

public class SignInEndpoint : Endpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok<int>, Ok>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IAuthCommonService _authCommonService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public SignInEndpoint(
        UserManager<AuthUser> userManager,
        IAuthCommonService authCommonService,
        IDateTimeService dateTimeService,
        IEmailService emailService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _authCommonService = authCommonService;
        _dateTimeService = dateTimeService;
        _emailService = emailService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok<int>, Ok>> ExecuteAsync(SignInCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
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

            return TypedResults.Ok((int)user.OtpTypeId!);
        }

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
