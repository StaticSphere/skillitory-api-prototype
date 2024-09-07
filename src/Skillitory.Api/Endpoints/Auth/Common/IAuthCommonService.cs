using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.Endpoints.Auth.Common;

public interface IAuthCommonService
{
    Task<AuthTokensResponse> GenerateAuthTokensAsync(AuthUser user, CancellationToken ct = default);
    void SetAuthCookies(AuthTokensResponse authTokensResponse, bool? persistedSignIn = null);
}