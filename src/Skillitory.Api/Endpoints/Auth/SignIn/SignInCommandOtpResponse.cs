using Skillitory.Api.DataStore.Entities.Auth.Enumerations;

namespace Skillitory.Api.Endpoints.Auth.SignIn;

public record SignInCommandOtpResponse
{
    public OtpTypeEnum OtpType { get; init; }
    public string UserUniqueKey { get; init; } = "";
}
