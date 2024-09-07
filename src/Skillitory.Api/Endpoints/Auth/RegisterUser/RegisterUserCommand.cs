namespace Skillitory.Api.Endpoints.Auth.RegisterUser;

public record RegisterUserCommand
{
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string? Organization { get; init; }
    public string Password { get; init; } = "";
}
