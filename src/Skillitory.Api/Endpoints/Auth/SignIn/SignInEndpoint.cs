using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Auth.Enumerations;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignIn;

public class SignInEndpoint : Endpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok<SignInCommandTokenResponse>, Ok<SignInCommandOtpResponse>>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public SignInEndpoint(
        UserManager<AuthUser> userManager,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        ITokenService tokenService,
        IEmailService emailService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _tokenService = tokenService;
        _emailService = emailService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<SignInCommandTokenResponse>, Ok<SignInCommandOtpResponse>>> ExecuteAsync(SignInCommand req, CancellationToken ct)
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

        return TypedResults.Ok(new SignInCommandTokenResponse
        {
            UserUniqueKey = user.UserUniqueKey,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiration = tokens.AccessTokenExpiration,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration,
        });
    }
}
