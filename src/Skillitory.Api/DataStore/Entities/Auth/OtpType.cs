using Skillitory.Api.DataStore.Common.Enumerations;

namespace Skillitory.Api.DataStore.Entities.Auth;

public class OtpType
{
    public OtpTypeEnum Id { get; set; }
    public string Name { get; set; } = "";
}
