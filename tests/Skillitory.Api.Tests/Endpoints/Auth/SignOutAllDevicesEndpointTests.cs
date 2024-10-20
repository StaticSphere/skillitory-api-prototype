using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Skillitory.Api.Endpoints.Auth.SignOutAllDevices;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class SignOutAllDevicesEndpointTests
{
    private readonly ITokenService _tokenService;
    private readonly ISignOutAllDevicesDataService _signOutAllDevicesDataService;
    private readonly SignOutAllDevicesEndpoint _endpoint;

    public SignOutAllDevicesEndpointTests()
    {
        _tokenService = Substitute.For<ITokenService>();
        _signOutAllDevicesDataService = Substitute.For<ISignOutAllDevicesDataService>();
        _endpoint = new SignOutAllDevicesEndpoint(_tokenService, _signOutAllDevicesDataService);
    }

    [Fact]
    public async Task ExecuteAsync_GetsPrincipalFromAccessToken()
    {
        var request = new SignOutAllDevicesCommand { AccessToken = "abc123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "123456")]));
        _tokenService.GetClaimsPrincipalFromAccessToken("123456").Returns(principal);

        await _endpoint.ExecuteAsync(request, default);

        _tokenService.Received(1).GetClaimsPrincipalFromAccessToken("abc123");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenPrincipalNotFound()
    {
        var request = new SignOutAllDevicesCommand { AccessToken = "abc123" };
        _tokenService.GetClaimsPrincipalFromAccessToken("123456").Returns((ClaimsPrincipal) null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ExecuteAsync_DeletesUserRefreshTokens_WhenPrincipalFound()
    {
        var request = new SignOutAllDevicesCommand { AccessToken = "abc123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "123456")]));
        _tokenService.GetClaimsPrincipalFromAccessToken("abc123").Returns(principal);

        await _endpoint.ExecuteAsync(request, default);

        await _signOutAllDevicesDataService.Received(1).DeleteAllUserRefreshTokensAsync("123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenPrincipalFound()
    {
        var request = new SignOutAllDevicesCommand { AccessToken = "abc123" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "123456")]));
        _tokenService.GetClaimsPrincipalFromAccessToken("abc123").Returns(principal);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();
    }
}
