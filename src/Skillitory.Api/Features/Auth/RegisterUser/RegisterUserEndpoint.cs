using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;
using Visus.Cuid;

namespace Skillitory.Api.Features.Auth.RegisterUser;

public class RegisterUserEndpoint : Endpoint<RegisterUserCommand, Results<NoContent, Conflict, StatusCodeHttpResult>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILoggerService<RegisterUserEndpoint> _loggerService;

    public RegisterUserEndpoint(
        UserManager<AuthUser> userManager,
        IEmailService emailService,
        IAuditService auditService,
        ILoggerService<RegisterUserEndpoint> loggerService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _auditService = auditService;
        _loggerService = loggerService;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, Conflict, StatusCodeHttpResult>> ExecuteAsync(RegisterUserCommand req, CancellationToken ct)
    {
        var user = new AuthUser
        {
            UserUniqueKey = new Cuid2().ToString(),
            Email = req.Email,
            UserName = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine,
                result.Errors.Select(x => $"Code: {x.Code}, Description: {x.Description}"));
            _loggerService.LogError("An error occurred trying to register a new user: {errors}", errors);

            if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                return TypedResults.Conflict();

            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationEmailAsync(user.Email, token, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.NewUserRegistered, ct);

        return TypedResults.NoContent();
    }
}
