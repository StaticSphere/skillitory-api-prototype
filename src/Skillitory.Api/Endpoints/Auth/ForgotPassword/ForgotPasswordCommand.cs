namespace Skillitory.Api.Endpoints.Auth.ForgotPassword;

public record ForgotPasswordCommand
{
    public string Email { get; init; } = "";
}
