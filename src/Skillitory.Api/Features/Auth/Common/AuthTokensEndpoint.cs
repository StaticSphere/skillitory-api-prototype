using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.Common;

public abstract class AuthTokensEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> where TRequest : notnull
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly ITokenService _tokenService;

    protected AuthTokensEndpoint(
        UserManager<AuthUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    protected async Task<AuthTokensResponse> GenerateAuthTokensAsync(AuthUser user, CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.UserUniqueKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokens = _tokenService.GenerateAuthTokens(claims);
        return new AuthTokensResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiration = tokens.RefreshTokenExpiration
        };
    }
}
