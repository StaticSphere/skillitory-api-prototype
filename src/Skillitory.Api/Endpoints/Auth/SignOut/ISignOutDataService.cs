namespace Skillitory.Api.Endpoints.Auth.SignOut;

public interface ISignOutDataService
{
    Task DeleteUserRefreshTokenAsync(string userUniqueKey, string refreshToken, CancellationToken cancellationToken = default);
}
