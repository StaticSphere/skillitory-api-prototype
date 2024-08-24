namespace Skillitory.Api.Features.Auth.SignOut;

public record SignOutCommand
{
    public string RefreshToken { get; init; } = "";
}
