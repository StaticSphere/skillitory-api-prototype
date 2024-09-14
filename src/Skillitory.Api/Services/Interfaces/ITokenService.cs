using System.Security.Claims;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models;

namespace Skillitory.Api.Services.Interfaces;

public interface ITokenService
{
    Task<TokenData> GenerateAuthTokensAsync(AuthUser user, Guid jti, CancellationToken ct = default);
    ClaimsPrincipal? GetClaimsPrincipalFromAccessToken(string accessToken);
}
