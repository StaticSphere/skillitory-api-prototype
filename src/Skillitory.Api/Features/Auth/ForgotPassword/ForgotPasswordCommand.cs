namespace Skillitory.Api.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand
{
    public string Email { get; init; } = "";
}
