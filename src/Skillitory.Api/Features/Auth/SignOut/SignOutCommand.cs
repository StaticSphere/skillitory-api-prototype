namespace Skillitory.Api.Features.Auth.SignOut;

public record SignOutCommand
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
}
