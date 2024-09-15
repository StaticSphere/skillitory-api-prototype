using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public class RefreshTokensEndpoint : Endpoint<RefreshTokensCommand, Results<UnauthorizedHttpResult, Ok<RefreshTokensCommandResponse>>>
{
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly IUserDataService _userDataService;
    private readonly ITokenService _tokenService;

    public RefreshTokensEndpoint(
        IUserRefreshTokenDataService userRefreshTokenDataService,
        IUserDataService userDataService,
        ITokenService tokenService)
    {
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _userDataService = userDataService;
        _tokenService = tokenService;
    }

    public override void Configure()
    {
        Post("/auth/refresh");
        AllowAnonymous();
    }

    public override async Task<Results<UnauthorizedHttpResult, Ok<RefreshTokensCommandResponse>>> ExecuteAsync(RefreshTokensCommand req, CancellationToken ct)
    {
        var currentUserRefreshToken =
            await _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync(req.RefreshToken, ct);

        if (currentUserRefreshToken is null)
            return TypedResults.Unauthorized();

        var user = await _userDataService.GetUserByIdAsync(currentUserRefreshToken.UserId, ct);
        if (user is null)
            return TypedResults.Unauthorized();

        var jti = Guid.NewGuid();
        var tokens = await _tokenService.GenerateAuthTokensAsync(user, jti, ct);

        await _userRefreshTokenDataService.UpdateUserRefreshTokenAsync(currentUserRefreshToken, jti, tokens.RefreshToken,
            tokens.RefreshTokenExpiration, ct);

        return TypedResults.Ok((RefreshTokensCommandResponse)tokens);
    }
}
