using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public interface ISignInOtpDataService
{
    Task<AuthUser?> GetUserByUserUniqueKeyAsync(string userUniqueKey, CancellationToken cancellationToken = default);
}
