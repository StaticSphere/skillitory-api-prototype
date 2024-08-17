using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignIn;

public class SignInEndpoint : AuthTokensEndpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok, Ok<AuthTokensResponse>, Ok<string>>>
{
    private readonly UserManager<SkillitoryUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public SignInEndpoint(
        UserManager<SkillitoryUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IEmailService emailService,
        IAuditService auditService,
        IOptions<SecurityConfiguration> securityConfiguration)
    : base(userManager, httpContextAccessor, tokenService, securityConfiguration.Value)
    {
        _userManager = userManager;
        _emailService = emailService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok, Ok<AuthTokensResponse>, Ok<string>>> ExecuteAsync(SignInCommand req, CancellationToken ct)
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

            return TypedResults.Ok(user.OtpTypeId.ToString());
        }

        var authTokenResponse = await GenerateAuthTokensAsync(user, req.UseCookie, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        return authTokenResponse is null
            ? TypedResults.Ok()
            : TypedResults.Ok(authTokenResponse);
    }
}
