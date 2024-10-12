using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.RefreshTokens;
using Skillitory.Api.Models;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class RefreshTokensEndpointTests
{
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly IUserDataService _userDataService;
    private readonly ITokenService _tokenService;
    private readonly RefreshTokensEndpoint _endpoint;

    public RefreshTokensEndpointTests()
    {
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _userDataService = Substitute.For<IUserDataService>();
        _tokenService = Substitute.For<ITokenService>();

        _endpoint = new RefreshTokensEndpoint(_userRefreshTokenDataService,
            _userDataService, _tokenService);
    }

    [Fact]
    public async Task ExecuteAsync_GetsCurrentUserRefreshToken()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };

        await _endpoint.ExecuteAsync(request, default);

        _userRefreshTokenDataService.Received(1).GetCurrentUserRefreshTokenAsync("123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenUserRefreshTokenNotFound()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns((UserRefreshToken?)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_GetsUserById()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);

        await _endpoint.ExecuteAsync(request, default);

        _userDataService.Received(1).GetUserByIdAsync(1);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenUserNotFound()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns((AuthUser?)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesAuthTokens()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        var user = new AuthUser { Id = 1 };
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns(user);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            RefreshToken = "789012", RefreshTokenExpiration = DateTime.Now.AddDays(7)
        });

        await _endpoint.ExecuteAsync(request, default);

        _tokenService.Received(1).GenerateAuthTokensAsync(user, Arg.Any<Guid>());
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesUserRefreshToken()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        var user = new AuthUser { Id = 1 };
        var refreshTokenExpiration = DateTime.Now.AddDays(7);
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns(user);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            RefreshToken = "789012", RefreshTokenExpiration = refreshTokenExpiration
        });

        await _endpoint.ExecuteAsync(request, default);

        _userRefreshTokenDataService.Received(1).UpdateUserRefreshTokenAsync(userRefreshToken,
            Arg.Any<Guid>(), "789012", refreshTokenExpiration);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOkWithTokens_WhenSuccessful()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        var user = new AuthUser { Id = 1 };
        var refreshTokenExpiration = DateTime.Now.AddDays(7);
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns(user);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            RefreshToken = "789012", RefreshTokenExpiration = refreshTokenExpiration
        });

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<RefreshTokensCommandResponse>>();
        var value = result.Result.As<Ok<RefreshTokensCommandResponse>>().Value;
        value.Should().BeEquivalentTo(new RefreshTokensCommandResponse
        {
            RefreshToken = "789012", RefreshTokenExpiration = refreshTokenExpiration
        });
    }
}
