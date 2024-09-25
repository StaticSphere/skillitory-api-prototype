using Skillitory.Api.DataStore.Entities.Auth.Enumerations;

namespace Skillitory.Api.DataStore.Entities.Auth;

public class OtpType
{
    public OtpTypeEnum Id { get; set; }
    public string Name { get; set; } = "";
}
