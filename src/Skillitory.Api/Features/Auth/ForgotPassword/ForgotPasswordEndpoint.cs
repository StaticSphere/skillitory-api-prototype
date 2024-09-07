using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.ForgotPassword;

public class ForgotPasswordEndpoint : Endpoint<ForgotPasswordCommand, NoContent>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public ForgotPasswordEndpoint(
        UserManager<AuthUser> userManager,
        IEmailService emailService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(ForgotPasswordCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.NoContent();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendResetPasswordEmailAsync(req.Email, token, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.ForgotPassword, ct);

        return TypedResults.NoContent();
    }
}
