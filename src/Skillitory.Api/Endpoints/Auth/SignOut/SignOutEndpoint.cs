using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;

namespace Skillitory.Api.Endpoints.Auth.SignOut;

public class SignOutEndpoint : Endpoint<SignOutCommand, NoContent>
{
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;

    public SignOutEndpoint(IUserRefreshTokenDataService userRefreshTokenDataService)
    {
        _userRefreshTokenDataService = userRefreshTokenDataService;
    }

    public override void Configure()
    {
        Post("/auth/sign-out");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(SignOutCommand req, CancellationToken ct)
    {
        await _userRefreshTokenDataService.DeleteUserRefreshTokenAsync(req.RefreshToken, ct);

        return TypedResults.NoContent();
    }
}
