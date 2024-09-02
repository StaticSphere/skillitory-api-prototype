namespace Skillitory.Api.Features.Auth.RegisterUser;

public interface IRegisterUserDataService
{
    Task<bool> GetOrganizationExistsAsync(string organization, CancellationToken cancellationToken = default);
}
