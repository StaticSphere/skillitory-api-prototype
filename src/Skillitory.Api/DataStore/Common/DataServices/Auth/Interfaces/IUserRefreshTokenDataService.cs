using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;

public interface IUserRefreshTokenDataService
{
    Task<UserRefreshToken?> GetCurrentUserRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task SaveNewUserRefreshTokenAsync(int userId, Guid jti, string refreshToken, DateTimeOffset refreshTokenExpiration,
        CancellationToken cancellationToken = default);

    Task UpdateUserRefreshTokenAsync(UserRefreshToken token, Guid jti, string refreshToken, DateTimeOffset refreshTokenExpiration,
        CancellationToken cancellationToken = default);

    Task DeleteUserRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
