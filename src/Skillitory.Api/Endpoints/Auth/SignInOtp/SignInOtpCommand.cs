using Skillitory.Api.DataStore.Entities.Auth.Enumerations;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public record SignInOtpCommand
{
    public string Email { get; init; } = "";
    public string Otp { get; init; } = "";
    public bool PersistedSignIn { get; init; }
    public OtpTypeEnum OtpType { get; init; }
}
