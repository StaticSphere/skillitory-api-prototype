namespace Skillitory.Api.Endpoints.Auth.SignOut;

public record SignOutCommand
{
    public string RefreshToken { get; init; } = "";
    public bool IsBrowser { get; init; }
}
