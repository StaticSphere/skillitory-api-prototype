using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public class RefreshTokensEndpoint : Endpoint<RefreshTokensCommand, Results<
    UnauthorizedHttpResult,
    Ok<RefreshTokensCommandAppResponse>,
    Ok<RefreshTokensCommandBrowserResponse>
>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly IUserDataService _userDataService;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly SecurityConfiguration _securityConfiguration;

    public RefreshTokensEndpoint(
        IHttpContextAccessor httpContextAccessor,
        IUserRefreshTokenDataService userRefreshTokenDataService,
        IUserDataService userDataService,
        ITokenService tokenService,
        ICookieService cookieService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRefreshTokenDataService = userRefreshTokenDataService;
        _userDataService = userDataService;
        _tokenService = tokenService;
        _cookieService = cookieService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public override void Configure()
    {
        Post("/auth/refresh");
        AllowAnonymous();
    }

    public override async Task<Results<
        UnauthorizedHttpResult,
        Ok<RefreshTokensCommandAppResponse>,
        Ok<RefreshTokensCommandBrowserResponse>
    >> ExecuteAsync(RefreshTokensCommand req, CancellationToken ct)
    {
        var refreshToken = req.RefreshToken;
        if (req.IsBrowser)
        {
            refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies[_securityConfiguration.RefreshCookieName] ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
            return TypedResults.Unauthorized();

        var currentUserRefreshToken =
            await _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync(refreshToken, ct);

        if (currentUserRefreshToken is null)
            return TypedResults.Unauthorized();

        var user = await _userDataService.GetUserByIdAsync(currentUserRefreshToken.UserId, ct);
        if (user is null)
            return TypedResults.Unauthorized();

        var jti = Guid.NewGuid();
        var tokens = await _tokenService.GenerateAuthTokensAsync(user, jti, ct);

        await _userRefreshTokenDataService.UpdateUserRefreshTokenAsync(currentUserRefreshToken, jti, tokens.RefreshToken,
            tokens.RefreshTokenExpiration, ct);

        if (!req.IsBrowser)
        {
            return TypedResults.Ok((RefreshTokensCommandAppResponse)tokens);
        }

        _cookieService.SetRefreshTokenCookie(tokens.RefreshToken, tokens.RefreshTokenExpiration);

        return TypedResults.Ok((RefreshTokensCommandBrowserResponse)tokens);
    }
}
