using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.ResetPassword;

public class ResetPasswordEndpoint : Endpoint<ResetPasswordCommand, Results<NotFound, StatusCodeHttpResult, UnprocessableEntity, NoContent>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IAuditService _auditService;

    public ResetPasswordEndpoint(
        UserManager<AuthUser> userManager,
        IAuditService auditService)
    {
        _userManager = userManager;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<Results<NotFound, StatusCodeHttpResult, UnprocessableEntity, NoContent>> ExecuteAsync(ResetPasswordCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.NotFound();

        var result = await _userManager.ResetPasswordAsync(user, req.Token, req.Password);
        if (!result.Succeeded)
        {
            return result.Errors.FirstOrDefault()?.Code switch
            {
                "InvalidToken" => TypedResults.StatusCode(StatusCodes.Status410Gone),
                { } err when err.StartsWith("Password") => TypedResults.UnprocessableEntity(),
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.ResetPassword, ct);

        return TypedResults.NoContent();
    }
}
