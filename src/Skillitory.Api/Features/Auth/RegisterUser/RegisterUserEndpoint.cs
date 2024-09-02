using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Mbr;
using Skillitory.Api.DataStore.Entities.Org;
using Skillitory.Api.Services.Interfaces;
using Visus.Cuid;

namespace Skillitory.Api.Features.Auth.RegisterUser;

public class RegisterUserEndpoint : Endpoint<RegisterUserCommand, Results<NoContent, Conflict<string>, StatusCodeHttpResult>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IRegisterUserDataService _registerUserDataService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILoggerService<RegisterUserEndpoint> _loggerService;

    public RegisterUserEndpoint(
        UserManager<AuthUser> userManager,
        IRegisterUserDataService registerUserDataService,
        IEmailService emailService,
        IAuditService auditService,
        ILoggerService<RegisterUserEndpoint> loggerService)
    {
        _userManager = userManager;
        _registerUserDataService = registerUserDataService;
        _emailService = emailService;
        _auditService = auditService;
        _loggerService = loggerService;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, Conflict<string>, StatusCodeHttpResult>> ExecuteAsync(RegisterUserCommand req, CancellationToken ct)
    {
        var user = new AuthUser
        {
            UserUniqueKey = new Cuid2().ToString(),
            Email = req.Email,
            UserName = req.Email,
            Member = new Member
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
            }
        };

        if (!string.IsNullOrWhiteSpace(req.Organization))
        {
            var organizationExists = await _registerUserDataService.GetOrganizationExistsAsync(req.Organization, ct);
            if (organizationExists)
                return TypedResults.Conflict("Organization already exists");

            user.Member.Organization = new Organization
            {
                OrganizationUniqueKey = new Cuid2().ToString(),
                Name = req.Organization,
            };
        }

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine,
                result.Errors.Select(x => $"Code: {x.Code}, Description: {x.Description}"));
            _loggerService.LogError("An error occurred trying to register a new user: {errors}", errors);

            if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                return TypedResults.Conflict("User account already exists");

            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationEmailAsync(user.Email, token, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.NewUserRegistered, ct);

        return TypedResults.NoContent();
    }
}
