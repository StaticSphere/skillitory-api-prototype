namespace Skillitory.Api.Features.Auth.RegisterUser;

public record RegisterUserCommand
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}
