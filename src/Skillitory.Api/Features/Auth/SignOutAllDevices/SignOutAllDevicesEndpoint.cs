using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Auth.SignOutAllDevices;

public class SignOutAllDevicesEndpoint : Endpoint<SignOutAllDevicesCommand, NoContent>
{
    private readonly ITokenService _tokenService;
    private readonly ISignOutAllDevicesDataService _signOutAllDevicesDataService;

    public SignOutAllDevicesEndpoint(
        ITokenService tokenService,
        ISignOutAllDevicesDataService signOutAllDevicesDataService)
    {
        _tokenService = tokenService;
        _signOutAllDevicesDataService = signOutAllDevicesDataService;
    }

    public override void Configure()
    {
        Post("/auth/sign-out-all-devices");
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(SignOutAllDevicesCommand req, CancellationToken ct)
    {
        var claimsPrincipal = _tokenService.GetClaimsPrincipalFromAccessToken(req.AccessToken);

        var userUniqueKey = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userUniqueKey)) return TypedResults.NoContent();

        await _signOutAllDevicesDataService.DeleteAllUserRefreshTokensAsync(userUniqueKey, ct);

        return TypedResults.NoContent();
    }
}
