namespace Skillitory.Api.Endpoints.Auth.ValidateUser;

public interface IValidateUserDataService
{
    Task EnableSignInAsync(int userId, CancellationToken cancellationToken = default);
}
