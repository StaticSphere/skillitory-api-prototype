namespace Skillitory.Api.Features.Auth.SignIn;

public record SignInCommand
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}
