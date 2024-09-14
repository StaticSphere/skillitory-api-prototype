using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;

public interface IUserRefreshTokenDataService
{
    Task SaveUserRefreshTokenAsync(int userId, Guid jti, string refreshToken, DateTimeOffset refreshTokenExpiration,
        CancellationToken cancellationToken = default);

    Task DeleteUserRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
