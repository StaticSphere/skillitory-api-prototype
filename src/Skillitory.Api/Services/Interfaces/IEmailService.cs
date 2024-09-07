namespace Skillitory.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendSignInOtpEmailAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task SendResetPasswordEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}
