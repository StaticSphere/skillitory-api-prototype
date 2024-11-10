using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.RefreshTokens;
using Skillitory.Api.Models;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class RefreshTokensEndpointTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly IUserDataService _userDataService;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly RefreshTokensEndpoint _endpoint;

    public RefreshTokensEndpointTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _userDataService = Substitute.For<IUserDataService>();
        _tokenService = Substitute.For<ITokenService>();
        _cookieService = Substitute.For<ICookieService>();
        var securityConfiguration = Substitute.For<IOptions<SecurityConfiguration>>();

        securityConfiguration.Value.Returns(new SecurityConfiguration
        {
            RefreshCookieName = "__refresh",
            AuthCookieDomain = "https://www.test.com"
        });

        _endpoint = new RefreshTokensEndpoint(
            _httpContextAccessor,
            _userRefreshTokenDataService,
            _userDataService,
            _tokenService,
            _cookieService,
            securityConfiguration);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenRefreshTokenEmpty(string refreshToken)
    {
        var request = new RefreshTokensCommand { RefreshToken = refreshToken };

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_GetsCurrentUserRefreshToken()
    {
        var request = new RefreshTokensCommand { RefreshToken = "123456" };

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).GetCurrentUserRefreshTokenAsync("123456");
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

        await _userDataService.Received(1).GetUserByIdAsync(1);
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
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = DateTime.Now.AddMinutes(3),
            RefreshTokenExpiration = DateTime.Now.AddDays(7)
        });

        await _endpoint.ExecuteAsync(request, default);

        await _tokenService.Received(1).GenerateAuthTokensAsync(user, Arg.Any<Guid>());
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
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = refreshTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        });

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).UpdateUserRefreshTokenAsync(userRefreshToken,
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
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = refreshTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        });

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<RefreshTokensCommandAppResponse>>();
        var value = result.Result.As<Ok<RefreshTokensCommandAppResponse>>().Value;
        value.Should().BeEquivalentTo(new RefreshTokensCommandAppResponse
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = refreshTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        });
    }

    [Fact]
    public async Task ExecuteAsync_SetsRefreshCookie_WhenIsBrowser()
    {
        var request = new RefreshTokensCommand { IsBrowser = true};
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        var user = new AuthUser { Id = 1 };
        var refreshTokenExpiration = DateTime.Now.AddDays(7);
        _httpContextAccessor.HttpContext?.Request.Cookies["__refresh"].Returns("123456");
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns(user);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = refreshTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        });

        await _endpoint.ExecuteAsync(request, default);

        _cookieService.Received(1).SetRefreshTokenCookie("789012", refreshTokenExpiration);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOkWithAccessToken_WhenIsBrowser()
    {
        var request = new RefreshTokensCommand { IsBrowser = true};
        var userRefreshToken = new UserRefreshToken { UserId = 1 };
        var user = new AuthUser { Id = 1 };
        var refreshTokenExpiration = DateTime.Now.AddDays(7);
        _httpContextAccessor.HttpContext?.Request.Cookies["__refresh"].Returns("123456");
        _userRefreshTokenDataService.GetCurrentUserRefreshTokenAsync("123456").Returns(userRefreshToken);
        _userDataService.GetUserByIdAsync(1).Returns(user);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = refreshTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        });

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<RefreshTokensCommandBrowserResponse>>();
        var value = result.Result.As<Ok<RefreshTokensCommandBrowserResponse>>().Value;
        value.Should().BeEquivalentTo(new RefreshTokensCommandBrowserResponse
        {
            AccessToken = "123456",
            AccessTokenExpiration = refreshTokenExpiration,
        });
    }
}
