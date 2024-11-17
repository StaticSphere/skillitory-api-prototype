using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.Endpoints.Auth.SignOut;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class SignOutEndpointTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ICookieService _cookieService;
    private readonly SignOutEndpoint _endpoint;

    public SignOutEndpointTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _cookieService = Substitute.For<ICookieService>();
        var securityConfiguration = Substitute.For<IOptions<SecurityConfiguration>>();

        securityConfiguration.Value.Returns(new SecurityConfiguration
        {
            RefreshCookieName = "__refresh"
        });

        _endpoint = new SignOutEndpoint(
            _httpContextAccessor,
            _userRefreshTokenDataService,
            _cookieService,
            securityConfiguration
            );
    }

    [Fact]
    public async Task ExecuteAsync_DeletesUserRefreshToken_WhenRefreshTokenPassedIn()
    {
        var request = new SignOutCommand { RefreshToken = "123456" };

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).DeleteUserRefreshTokenAsync("123456");
    }

    [Fact]
    public async Task ExecuteAsync_DeletesUserRefreshToken_WhenIsBrowser()
    {
        var request = new SignOutCommand { IsBrowser = true};
        _httpContextAccessor.HttpContext.Request.Cookies["__refresh"].Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).DeleteUserRefreshTokenAsync("123456");
    }

    [Fact]
    public async Task ExecuteAsync_ClearsRefreshTokenCookie_WhenIsBrowser()
    {
        var request = new SignOutCommand { IsBrowser = true};
        _httpContextAccessor.HttpContext.Request.Cookies["__refresh"].Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        _cookieService.Received(1).ClearRefreshTokenCookie();
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotClearRefreshTokenCookie_WhenNotIsBrowser()
    {
        var request = new SignOutCommand { RefreshToken = "123456" };
        _httpContextAccessor.HttpContext.Request.Cookies["__refresh"].Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        _cookieService.DidNotReceive().ClearRefreshTokenCookie();
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
