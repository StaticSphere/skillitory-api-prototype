using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignOut;

public class SignOutEndpoint : Endpoint<SignOutCommand, Ok>
{
    private readonly ISignOutDataService _signOutDataService;
    private readonly IPrincipalService _principalService;

    public SignOutEndpoint(ISignOutDataService signOutDataService, IPrincipalService principalService)
    {
        _signOutDataService = signOutDataService;
        _principalService = principalService;
    }

    public override void Configure()
    {
        Post("/auth/sign-out");
    }

    public override async Task<Ok> ExecuteAsync(SignOutCommand req, CancellationToken ct)
    {
        await _signOutDataService.DeleteUserRefreshTokenAsync(_principalService.UserUniqueKey, req.RefreshToken, ct);

        return TypedResults.Ok();
    }
}
