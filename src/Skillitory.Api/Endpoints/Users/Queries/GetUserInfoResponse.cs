namespace Skillitory.Api.Endpoints.Users.Queries;

public record GetUserInfoResponse
{
    public string UserUniqueKey { get; init; } = "";
    public string? OrganizationUniqueKey { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string? Organization { get; init; }
    public string? Department { get; init; }
    public string? Title { get; init; }
    public long? AvatarVersionKey { get; init; }
    public string Role { get; init; } = "";
}
