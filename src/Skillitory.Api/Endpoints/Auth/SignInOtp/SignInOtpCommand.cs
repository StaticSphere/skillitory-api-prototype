using Skillitory.Api.DataStore.Common.Enumerations;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public record SignInOtpCommand
{
    public string Email { get; init; } = "";
    public string Otp { get; init; } = "";
    public bool PersistedSignIn { get; init; }
    public bool UseCookies { get; init; }

    public OtpTypeEnum OtpType { get; init; }
}
