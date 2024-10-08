namespace Skillitory.Api.Endpoints.Auth.ValidateUser;

public record ValidateUserCommand
{
    public string Email { get; init; } = "";
    public string Token { get; init; } = "";
}
