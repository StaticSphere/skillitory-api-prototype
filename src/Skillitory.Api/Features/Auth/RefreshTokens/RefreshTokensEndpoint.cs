using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Features.Auth.Common;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.RefreshTokens;

public class RefreshTokensEndpoint : AuthTokensEndpoint<RefreshTokensCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>>>
{
    private readonly UserManager<SkillitoryUser> _userManager;
    private readonly IRefreshTokensDataService _refreshTokensDataService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeService _dateTimeService;

    public RefreshTokensEndpoint(
        UserManager<SkillitoryUser> userManager,
        IRefreshTokensDataService refreshTokensDataService,
        ITokenService tokenService,
        IDateTimeService dateTimeService)
        : base(userManager, tokenService)
    {
        _userManager = userManager;
        _refreshTokensDataService = refreshTokensDataService;
        _tokenService = tokenService;
        _dateTimeService = dateTimeService;
    }

    public override void Configure()
    {
        Post("/auth/refresh-tokens");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>>> ExecuteAsync(RefreshTokensCommand req, CancellationToken ct)
    {
        var principal = _tokenService.GetClaimsPrincipalFromAccessToken(req.AccessToken);
        if (principal is null)
            return TypedResults.Unauthorized();

        var email = principal.Identity!.Name;
        var dbRefreshToken = await _refreshTokensDataService.GetUserRefreshTokenAsync(email!, req.RefreshToken, ct);
        if (dbRefreshToken is null || dbRefreshToken.ExpirationDateTime <= _dateTimeService.UtcNow)
            return TypedResults.Unauthorized();

        var user = await _userManager.FindByEmailAsync(email!);
        if (user is null || !user.IsSignInAllowed || user.TerminatedOn.HasValue)
            return TypedResults.Unauthorized();

        var authTokenResponse = await GenerateAuthTokensAsync(user, ct);
        dbRefreshToken.Token = authTokenResponse.RefreshToken;
        dbRefreshToken.ExpirationDateTime = authTokenResponse.RefreshTokenExpiration;
        dbRefreshToken.CreatedDateTime = _dateTimeService.UtcNow;
        await _refreshTokensDataService.UpdateUserRefreshTokenAsync(dbRefreshToken, ct);

        return TypedResults.Ok(authTokenResponse);
    }
}
