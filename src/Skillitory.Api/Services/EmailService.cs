using System.Web;
using FluentEmail.Core;
using Skillitory.Api.Resources;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IFluentEmail _mailer;

    public EmailService(IFluentEmail mailer, IConfiguration configuration)
    {
        _mailer = mailer;
        _configuration = configuration;
    }

    public async Task SendEmailConfirmationEmailAsync(string email, string token,
        CancellationToken cancellationToken = default)
    {
        var callbackUrl =
            $"{UrlRoot}auth/validate-email?token={HttpUtility.UrlEncode(token)}";
        await SendEmailInternalAsync(email, "Skillitory - Validate Skillitory Account", "ValidateSkillitoryAccount",
            new { callbackUrl }, cancellationToken);
    }

    internal string UrlRoot =>
        _configuration["WebAppUrl"]!.Trim('/', '\\') + "/";

    private async Task SendEmailInternalAsync(string email, string subject, string template, object? parameters,
        CancellationToken cancellationToken = default)
    {
        await _mailer.To(email)
            .Subject(subject)
            .UsingTemplateFromEmbedded(
                $"{EmbeddedResources.EmailTemplatePathRoot}.{template}.liquid",
                parameters,
                typeof(EmailService).Assembly)
            .SendAsync(cancellationToken);
    }
}
