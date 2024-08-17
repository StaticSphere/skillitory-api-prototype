namespace Skillitory.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}
