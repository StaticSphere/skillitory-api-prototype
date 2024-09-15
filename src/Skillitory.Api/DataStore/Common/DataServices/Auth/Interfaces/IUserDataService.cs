using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;

public interface IUserDataService
{
    Task<AuthUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
}
