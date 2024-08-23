using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.Features.Auth.RefreshTokens;

public interface IRefreshTokensDataService
{
    Task<UserRefreshToken?> GetUserRefreshTokenAsync(string email, string refreshToken, CancellationToken cancellationToken = default);
    Task UpdateUserRefreshTokenAsync(UserRefreshToken token, CancellationToken cancellationToken = default);
}