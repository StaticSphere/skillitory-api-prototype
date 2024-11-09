using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Auth.Enumerations;
using Skillitory.Api.Endpoints.Auth.SignIn;
using Skillitory.Api.Models;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class SignInEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;
    private readonly SignInEndpoint _endpoint;

    public SignInEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _tokenService = Substitute.For<ITokenService>();
        _emailService = Substitute.For<IEmailService>();
        _dateTimeService = Substitute.For<IDateTimeService>();
        _auditService = Substitute.For<IAuditService>();
        var hostEnvironment = Substitute.For<IHostEnvironment>();
        var securityConfiguration = Substitute.For<IOptions<SecurityConfiguration>>();

        hostEnvironment.EnvironmentName.Returns(Environments.Production);

        securityConfiguration.Value.Returns(new SecurityConfiguration
        {
            RefreshCookieName = "__refresh",
            AuthCookieDomain = "https://www.test.com"
        });

        _endpoint = new SignInEndpoint(
            _userManager,
            _httpContextAccessor,
            _userRefreshTokenDataService,
            _tokenService,
            _emailService,
            _dateTimeService,
            _auditService,
            hostEnvironment,
            securityConfiguration);
    }

    [Fact]
    public async Task ExecuteAsync_TrysToFindUserByEmail()
    {
        var request = new SignInCommand { Email = "test@test.com" };

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).FindByEmailAsync("test@test.com");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserNotFound()
    {
        var request = new SignInCommand { Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns((AuthUser?)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserNotAllowedSignIn()
    {
        var request = new SignInCommand { Email = "test@test.com" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = false };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserIsTerminated()
    {
        var request = new SignInCommand { Email = "test@test.com" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true, TerminatedOnDateTime = DateTimeOffset.UtcNow };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_ChecksPassword()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(false);

        await _endpoint.ExecuteAsync(request, default);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenPasswordCheckFails()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(false);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesMFAToken_WhenOtpTypeIsEmail()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true, TwoFactorEnabled = true, OtpTypeId = OtpTypeEnum.Email};
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
    }

    [Fact]
    public async Task ExecuteAsync_SendsOtpEmail_WhenOtpTypeIsEmail()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true, TwoFactorEnabled = true, OtpTypeId = OtpTypeEnum.Email};
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        await _emailService.Received(1).SendSignInOtpEmailAsync("test@test.com", "123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOk_WhenOtpTypeIsEmail()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", UserUniqueKey = "User123", IsSignInAllowed = true,
            TwoFactorEnabled = true, OtpTypeId = OtpTypeEnum.Email};
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider).Returns("123456");

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<SignInCommandOtpResponse>>()
            .Which.Value.Should().NotBeNull()
            .And.Match<SignInCommandOtpResponse>(r =>
                r.OtpType == OtpTypeEnum.Email
                && r.UserUniqueKey == "User123");
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotSendOtpEmail_WhenOtpTypeIsNotEmail()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true,
            TwoFactorEnabled = true, OtpTypeId = OtpTypeEnum.TimeBased };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        await _emailService.DidNotReceive().SendSignInOtpEmailAsync("test@test.com", "123456");
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesAuthTokens_WhenNotOtp()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _tokenService.Received(1).GenerateAuthTokensAsync(user, Arg.Any<Guid>());
    }

    [Fact]
    public async Task ExecuteAsync_SavesRefreshToken_WhenNotOtp()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).SaveNewUserRefreshTokenAsync(1,
            Arg.Any<Guid>(), "456", date);
    }

    [Fact]
    public async Task ExecuteAsync_AuditsSignIn_WhenNotOtp()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _auditService.Received(1).AuditUserActionAsync(1, AuditLogTypeEnum.SignIn);
    }

    [Fact]
    public async Task ExecuteAsync_SetsLastSignInDate_WhenNotOtp()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
        _dateTimeService.UtcNow.Returns(date);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).UpdateAsync(Arg.Is<AuthUser>(u => u.LastSignInDateTime == date));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOk_WhenNotOtp()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password" };
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
        _dateTimeService.UtcNow.Returns(date);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<SignInCommandAppResponse>>()
            .Which.Value.Should().NotBeNull()
            .And.BeEquivalentTo(new SignInCommandAppResponse
            {
                UserUniqueKey = "abc123",
                AccessToken = "123",
                RefreshToken = "456",
                AccessTokenExpiration = date,
                RefreshTokenExpiration = date
            });
    }

    [Fact]
    public async Task ExecuteAsync_SetsRefreshCookie_WhenIsBrowser()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password", IsBrowser = true};
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        });
        _dateTimeService.UtcNow.Returns(date);

        await _endpoint.ExecuteAsync(request, default);

        _httpContextAccessor.HttpContext?.Response.Cookies.Received(1).Append("__refresh",
            "789012", Arg.Do<CookieOptions>(x => x.Should().BeEquivalentTo(new CookieOptions
            {
                Expires = date,
                Domain = "www.test.com",
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            })));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOkWithAccessToken_WhenIsBrowser()
    {
        var request = new SignInCommand { Email = "test@test.com", Password = "password", IsBrowser = true};
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "password").Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        });
        _dateTimeService.UtcNow.Returns(date);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<SignInCommandBrowserResponse>>();
        var value = result.Result.As<Ok<SignInCommandBrowserResponse>>().Value;
        value.Should().BeEquivalentTo(new SignInCommandBrowserResponse
        {
            UserUniqueKey = "abc123",
            AccessToken = "123456",
            AccessTokenExpiration = date,
        });
    }
}
