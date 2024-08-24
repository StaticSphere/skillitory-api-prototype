using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignOut;

public class SignOutEndpoint : Endpoint<SignOutCommand, NoContent>
{
    private readonly ITokenService _tokenService;
    private readonly ISignOutDataService _signOutDataService;
    public SignOutEndpoint(ITokenService tokenService, ISignOutDataService signOutDataService)
    {
        _tokenService = tokenService;
        _signOutDataService = signOutDataService;
    }

    public override void Configure()
    {
        Post("/auth/sign-out");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(SignOutCommand req, CancellationToken ct)
    {
        var claimsPrincipal = _tokenService.GetClaimsPrincipalFromAccessToken(req.AccessToken);

        var userUniqueKey = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userUniqueKey)) return TypedResults.NoContent();

        await _signOutDataService.DeleteUserRefreshTokenAsync(userUniqueKey, req.RefreshToken, ct);

        return TypedResults.NoContent();
    }
}
