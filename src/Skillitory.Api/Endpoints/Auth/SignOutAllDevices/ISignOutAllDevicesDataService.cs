namespace Skillitory.Api.Endpoints.Auth.SignOutAllDevices;

public interface ISignOutAllDevicesDataService
{
    Task DeleteAllUserRefreshTokensAsync(string userUniqueKey, CancellationToken cancellationToken = default);
}
