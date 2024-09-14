using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignIn;

public class SignInEndpoint : Endpoint<SignInCommand, Results<UnauthorizedHttpResult, Ok<SignInCommandResponse>>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;

    public SignInEndpoint(
        UserManager<AuthUser> userManager,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        ITokenService tokenService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _tokenService = tokenService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        Post("/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<SignInCommandResponse>>> ExecuteAsync(SignInCommand req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.Unauthorized();

        if (!(await _userManager.CheckPasswordAsync(user, req.Password)))
            return TypedResults.Unauthorized();

        var jti = Guid.NewGuid();
        var tokens = await _tokenService.GenerateAuthTokensAsync(user, jti, ct);

        await _userRefreshTokenDataService.SaveUserRefreshTokenAsync(user.Id, jti, tokens.RefreshToken, tokens.RefreshTokenExpiration, ct);

        await _auditService.AuditUserActionAsync(user.Id, AuditLogTypeEnum.SignIn, ct);

        return TypedResults.Ok((SignInCommandResponse)tokens);
    }
}
