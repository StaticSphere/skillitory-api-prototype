using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Endpoints.Auth.SignOut;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class SignOutEndpointTests
{
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly SignOutEndpoint _endpoint;

    public SignOutEndpointTests()
    {
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _endpoint = new SignOutEndpoint(_userRefreshTokenDataService);
    }

    [Fact]
    public async Task ExecuteAsync_DeletesUserRefreshToken()
    {
        var request = new SignOutCommand { RefreshToken = "123456" };
        await _endpoint.ExecuteAsync(request, default);

        _userRefreshTokenDataService.Received(1).DeleteUserRefreshTokenAsync("123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent()
    {
        var request = new SignOutCommand { RefreshToken = "123456" };
        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();
    }
}
