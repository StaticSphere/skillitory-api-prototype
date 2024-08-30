using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignIn;

public class SignInEndpoint : AuthTokensEndpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok<int>>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;

    public SignInEndpoint(
        UserManager<AuthUser> userManager,
        ITokenService tokenService,
        IDateTimeService dateTimeService,
        IEmailService emailService,
        IAuditService auditService)
    : base(userManager, tokenService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _dateTimeService = dateTimeService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>, Ok<int>>> ExecuteAsync(SignInCommand req, CancellationToken ct)
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

        return TypedResults.Ok(authTokenResponse);
    }
}
