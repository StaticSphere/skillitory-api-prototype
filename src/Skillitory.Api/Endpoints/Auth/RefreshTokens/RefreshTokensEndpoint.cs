using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.Common;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public class RefreshTokensEndpoint : Endpoint<RefreshTokensCommand, Results<UnauthorizedHttpResult, Ok<AuthTokensResponse>>>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IRefreshTokensDataService _refreshTokensDataService;
    private readonly IAuthCommonService _authCommonService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeService _dateTimeService;

    public RefreshTokensEndpoint(
        UserManager<AuthUser> userManager,
        IRefreshTokensDataService refreshTokensDataService,
        IAuthCommonService authCommonService,
        ITokenService tokenService,
        IDateTimeService dateTimeService)
    {
        _userManager = userManager;
        _refreshTokensDataService = refreshTokensDataService;
        _authCommonService = authCommonService;
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

        var authTokenResponse = await _authCommonService.GenerateAuthTokensAsync(user, ct);
        dbRefreshToken.Token = authTokenResponse.RefreshToken;
        dbRefreshToken.ExpirationDateTime = authTokenResponse.RefreshTokenExpiration;
        dbRefreshToken.CreatedDateTime = _dateTimeService.UtcNow;
        await _refreshTokensDataService.UpdateUserRefreshTokenAsync(dbRefreshToken, ct);

        _authCommonService.SetAuthCookies(authTokenResponse);

        return TypedResults.Ok(authTokenResponse);
    }
}
