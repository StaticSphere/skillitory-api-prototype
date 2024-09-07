namespace Skillitory.Api.Endpoints.Auth.ResetPassword;

public record ResetPasswordCommand
{
    public string Token { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}
