using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services;

namespace Skillitory.Api.Tests.Services;

public class CookieServiceTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CookieService _serviceDev;
    private readonly CookieService _serviceProd;

    public CookieServiceTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var hostEnvironmentDev = Substitute.For<IHostEnvironment>();
        var hostEnvironmentProd = Substitute.For<IHostEnvironment>();
        var securityConfiguration = Substitute.For<IOptions<SecurityConfiguration>>();

        securityConfiguration.Value.Returns(new SecurityConfiguration
        {
            PersistSignInCookieName = "__persisted",
            RefreshCookieName = "__refresh",
            AuthCookieDomain = "www.test.com"
        });

        hostEnvironmentDev.EnvironmentName.Returns(Environments.Development);
        hostEnvironmentProd.EnvironmentName.Returns(Environments.Production);

        _serviceDev = new CookieService(
            _httpContextAccessor,
            securityConfiguration,
            hostEnvironmentDev);

        _serviceProd = new CookieService(
            _httpContextAccessor,
            securityConfiguration,
            hostEnvironmentProd);
    }

    [Fact]
    public void SetRefreshTokenCookie_DoesNotSetExpiresIfNotPersistant()
    {
        CookieOptions? options = null;
        _httpContextAccessor.HttpContext?.Request.Cookies["__persisted"].Returns(string.Empty);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("__refresh",
            "789012", Arg.Do<CookieOptions>(x => options = x));
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        _serviceProd.SetRefreshTokenCookie("789012", refreshTokenExpiration);

        _httpContextAccessor.HttpContext?.Response.Cookies.Received(1).Append("__refresh",
            "789012", Arg.Any<CookieOptions>());

        options.Should().NotBeNull();
        options.Should().BeEquivalentTo(new CookieOptions
        {
            Expires = null,
            Domain = "www.test.com",
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });
    }

    [Fact]
    public void SetRefreshTokenCookie_SetsExpiresIfPersistant()
    {
        CookieOptions? options = null;
        _httpContextAccessor.HttpContext?.Request.Cookies["__persisted"].Returns("true");
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("__refresh",
            "789012", Arg.Do<CookieOptions>(x => options = x));
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        _serviceProd.SetRefreshTokenCookie("789012", refreshTokenExpiration);

        _httpContextAccessor.HttpContext?.Response.Cookies.Received(1).Append("__refresh",
            "789012", Arg.Any<CookieOptions>());

        options.Should().NotBeNull();
        options.Should().BeEquivalentTo(new CookieOptions
        {
            Expires = refreshTokenExpiration,
            Domain = "www.test.com",
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });
    }

    [Fact]
    public void SetRefreshTokenCookie_DoesNotSetDomainOrSecureIfInDevelopmentEnvironment()
    {
        CookieOptions? options = null;
        _httpContextAccessor.HttpContext?.Request.Cookies["__persisted"].Returns(string.Empty);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("__refresh",
            "789012", Arg.Do<CookieOptions>(x => options = x));
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        _serviceDev.SetRefreshTokenCookie("789012", refreshTokenExpiration);

        _httpContextAccessor.HttpContext?.Response.Cookies.Received(1).Append("__refresh",
            "789012", Arg.Any<CookieOptions>());

        options.Should().NotBeNull();
        options.Should().BeEquivalentTo(new CookieOptions
        {
            Expires = null,
            Domain = null,
            Path = "/",
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });
    }
}
