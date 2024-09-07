namespace Skillitory.Api.Endpoints.Auth.SignOutAllDevices;

public record SignOutAllDevicesCommand
{
    public string AccessToken { get; init; } = "";
}
