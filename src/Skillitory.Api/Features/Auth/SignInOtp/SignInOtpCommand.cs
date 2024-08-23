using Skillitory.Api.DataStore.Common.Enumerations;

namespace Skillitory.Api.Features.Auth.SignInOtp;

public record SignInOtpCommand
{
    public string Email { get; init; } = "";
    public string Otp { get; init; } = "";
    public OtpTypeEnum OtpType { get; init; }
}
