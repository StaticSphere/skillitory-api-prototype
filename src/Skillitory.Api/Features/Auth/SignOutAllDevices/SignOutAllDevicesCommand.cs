namespace Skillitory.Api.Features.Auth.SignOutAllDevices;

public record SignOutAllDevicesCommand
{
    public string AccessToken { get; init; } = "";
}
