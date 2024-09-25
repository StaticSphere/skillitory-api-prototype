using System.Web;
using FluentEmail.Core;
using Fluid;
using Skillitory.Api.DataStore.Common.DataServices.Com.Interfaces;
using Skillitory.Api.DataStore.Entities.Com.Enumerations;
using Skillitory.Api.Exceptions;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

public class EmailService : IEmailService
{
    private static readonly FluidParser Parser = new ();
    private readonly ICommunicationTemplateDataService _communicationTemplateDataService;
    private readonly IConfiguration _configuration;
    private readonly IFluentEmail _mailer;

    public EmailService(
        ICommunicationTemplateDataService communicationTemplateDataService,
        IFluentEmail mailer,
        IConfiguration configuration)
    {
        _communicationTemplateDataService = communicationTemplateDataService;
        _mailer = mailer;
        _configuration = configuration;
    }

    public async Task SendEmailConfirmationEmailAsync(string email, string token,
        CancellationToken cancellationToken = default)
    {
        var callbackUrl =
            $"{UrlRoot}auth/validate?token={HttpUtility.UrlEncode(token)}";
        await SendEmailInternalAsync(email, "Skillitory - Validate Skillitory Account", "ValidateSkillitoryAccount",
            new { callbackUrl }, cancellationToken);
    }

    public async Task SendSignInOtpEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(email, "Skillitory - Sign In One Time Password", "SignInOtp", new { otp },
            cancellationToken);
    }

    public async Task SendResetPasswordEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var callbackUrl =
            $"{UrlRoot}auth/reset-password?token={HttpUtility.UrlEncode(token)}";
        await SendEmailInternalAsync(email, "Skillitory - Forgot Password", "ForgotPassword", new { callbackUrl },
            cancellationToken);
    }

    internal string UrlRoot =>
        _configuration["WebAppUrl"]!.Trim('/', '\\') + "/";

    private async Task SendEmailInternalAsync(string email, string subject, string templateName, object? parameters,
        CancellationToken cancellationToken = default)
    {
        var template = await _communicationTemplateDataService.GetCommunicationTemplateAsync(templateName,
            CommunicationTemplateTypeEnum.Email, cancellationToken);

        if (template is null)
            throw new EmailTemplateNotFoundException(templateName);

        string renderedMessage = "";
        if (Parser.TryParse(template, out var parsedTemplate, out var error))
        {
            var context = new TemplateContext(parameters);
            renderedMessage = await parsedTemplate.RenderAsync(context);
        }
        else
        {
            throw new EmailTemplateRenderException(templateName, error);
        }

        await _mailer.To(email)
            .Subject(subject)
            .Body(renderedMessage, true)
            .SendAsync(cancellationToken);
    }
}
