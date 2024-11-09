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
using Skillitory.Api.Endpoints.Auth.SignInOtp;
using Skillitory.Api.Models;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class SignInOtpEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISignInOtpDataService _signInOtpDataService;
    private readonly IUserRefreshTokenDataService _userRefreshTokenDataService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IAuditService _auditService;
    private readonly SignInOtpEndpoint _endpoint;

    public SignInOtpEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _signInOtpDataService = Substitute.For<ISignInOtpDataService>();
        _userRefreshTokenDataService = Substitute.For<IUserRefreshTokenDataService>();
        _tokenService = Substitute.For<ITokenService>();
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

        _endpoint = new SignInOtpEndpoint(
            _userManager,
            _httpContextAccessor,
            _signInOtpDataService,
            _userRefreshTokenDataService,
            _tokenService,
            _dateTimeService,
            _auditService,
            hostEnvironment,
            securityConfiguration);
    }

    [Fact]
    public async Task ExecuteAsync_TrysToFindUserByUniqueKey()
    {
        var request = new SignInOtpCommand { UserUniqueKey = "abc123" };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns((AuthUser)null!);

        await _endpoint.ExecuteAsync(request, default);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenUserNotFound()
    {
        var request = new SignInOtpCommand { UserUniqueKey = "abc123" };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns((AuthUser)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenUserNotAllowedSignIn()
    {
        var request = new SignInOtpCommand { UserUniqueKey = "abc123" };
        var user = new AuthUser { UserUniqueKey = "abc123", IsSignInAllowed = false };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenUserIsTerminated()
    {
        var request = new SignInOtpCommand { UserUniqueKey = "abc123" };
        var user = new AuthUser { UserUniqueKey = "abc123", IsSignInAllowed = true, TerminatedOnDateTime = DateTimeOffset.UtcNow };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_VerifiesEmailMfaToken()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { UserUniqueKey = "abc123", IsSignInAllowed = true };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(false);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).VerifyTwoFactorTokenAsync(user,
            TokenOptions.DefaultEmailProvider, "123456");
    }

    [Fact]
    public async Task ExecuteAsync_VerifiesTimeBasedMfaToken()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.TimeBased,
            Otp = "123456"
        };
        var user = new AuthUser { UserUniqueKey = "abc123", IsSignInAllowed = true };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, "123456")
            .Returns(false);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).VerifyTwoFactorTokenAsync(user,
            TokenOptions.DefaultAuthenticatorProvider, "123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnauthorized_WhenOtpNotVerified()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { UserUniqueKey = "abc123", IsSignInAllowed = true };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(false);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesAuthTokens()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _tokenService.Received(1).GenerateAuthTokensAsync(user, Arg.Any<Guid>());
    }

    [Fact]
    public async Task ExecuteAsync_SavesRefreshToken()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _userRefreshTokenDataService.Received(1).SaveNewUserRefreshTokenAsync(1,
            Arg.Any<Guid>(), "456", date);
    }

    [Fact]
    public async Task ExecuteAsync_AuditsSignIn()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);

        await _endpoint.ExecuteAsync(request, default);

        await _auditService.Received(1).AuditUserActionAsync(1, AuditLogTypeEnum.SignIn);
    }

    [Fact]
    public async Task ExecuteAsync_SetsLastSignInDate()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
        _dateTimeService.UtcNow.Returns(date);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).UpdateAsync(Arg.Is<AuthUser>(u => u.LastSignInDateTime == date));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsOk()
    {
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456"
        };
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123",
            RefreshToken = "456",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
        _dateTimeService.UtcNow.Returns(date);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<SignInOtpCommandAppResponse>>()
            .Which.Value.Should().NotBeNull()
            .And.BeEquivalentTo(new SignInOtpCommandAppResponse
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
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456",
            IsBrowser = true
        };
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
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
        var request = new SignInOtpCommand
        {
            UserUniqueKey = "abc123",
            OtpType = OtpTypeEnum.Email,
            Otp = "123456",
            IsBrowser = true
        };
        var user = new AuthUser { Id = 1, UserUniqueKey = "abc123", Email = "test@test.com", IsSignInAllowed = true };
        var date = DateTimeOffset.UtcNow;
        var tokenData = new TokenData
        {
            AccessToken = "123456",
            RefreshToken = "789012",
            AccessTokenExpiration = date,
            RefreshTokenExpiration = date
        };
        _signInOtpDataService.GetUserByUserUniqueKeyAsync("abc123").Returns(user);
        _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456")
            .Returns(true);
        _tokenService.GenerateAuthTokensAsync(user, Arg.Any<Guid>()).Returns(tokenData);
        _dateTimeService.UtcNow.Returns(date);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<SignInOtpCommandBrowserResponse>>();
        var value = result.Result.As<Ok<SignInOtpCommandBrowserResponse>>().Value;
        value.Should().BeEquivalentTo(new SignInOtpCommandBrowserResponse
        {
            UserUniqueKey = "abc123",
            AccessToken = "123456",
            AccessTokenExpiration = date,
        });
    }
}
