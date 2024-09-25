using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.ValidateUser;

public class ValidateUserEndpoint : Endpoint<ValidateUserCommand, Results<NotFound, UnprocessableEntity, NoContent>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IValidateUserDataService _validateUserDataService;
    private readonly IAuditService _auditService;

    public ValidateUserEndpoint(
        UserManager<AuthUser> userManager,
        IValidateUserDataService validateUserDataService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _validateUserDataService = validateUserDataService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/validate");
        AllowAnonymous();
    }

    public override async Task<Results<NotFound, UnprocessableEntity, NoContent>> ExecuteAsync(ValidateUserCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
            return TypedResults.NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, req.Token);

        if (!result.Succeeded)
            return TypedResults.UnprocessableEntity();

        await _validateUserDataService.EnableSignInAsync(user.Id, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.NewUserEmailVerified, ct);

        return TypedResults.NoContent();
    }
}
