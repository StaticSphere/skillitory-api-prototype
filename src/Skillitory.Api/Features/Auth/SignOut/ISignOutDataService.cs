namespace Skillitory.Api.Features.Auth.SignOut;

public interface ISignOutDataService
{
    Task DeleteUserRefreshTokenAsync(string userUniqueKey, string refreshToken, CancellationToken cancellationToken = default);
}
