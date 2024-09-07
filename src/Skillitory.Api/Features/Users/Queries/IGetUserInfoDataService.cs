namespace Skillitory.Api.Features.Users.Queries;

public interface IGetUserInfoDataService
{
    Task<GetUserInfoResponse?> GetUserInfoAsync(string userUniqueKey, CancellationToken cancellationToken = default);
}
