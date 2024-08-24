namespace Skillitory.Api.Features.Auth.SignOutAllDevices;

public interface ISignOutAllDevicesDataService
{
    Task DeleteAllUserRefreshTokensAsync(string userUniqueKey, CancellationToken cancellationToken = default);
}
