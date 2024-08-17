namespace Skillitory.Api.Features.Auth.ValidateUser;

public interface IValidateUserDataService
{
    Task EnableSignInAsync(int userId, CancellationToken cancellationToken = default);
}
