using System.Diagnostics.CodeAnalysis;

namespace Skillitory.Api.Models.Configuration;

[ExcludeFromCodeCoverage]
public record SmtpConfiguration
{
    public string DefaultSender { get; init; } = "";
    public string Server { get; init; } = "";
    public int Port { get; init; } = 25;
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public bool UseSsl { get; init; }
    public bool UsePickupDirectory { get; init; }
    public string? MailPickupDirectory { get; init; }
}
