using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.ForgotPassword;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class ForgotPasswordEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly AuthUser _user;
    private readonly ForgotPasswordEndpoint _endpoint;

    public ForgotPasswordEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _emailService = Substitute.For<IEmailService>();
        _auditService = Substitute.For<IAuditService>();

        _endpoint = new ForgotPasswordEndpoint(_userManager,
            _emailService, _auditService);

        _user = new AuthUser
        {
            Id = 1,
            UserUniqueKey = "user_key",
            IsSignInAllowed = true
        };
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenUserNotFound()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns((AuthUser)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenUserNotAllowedSignIn()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _user.IsSignInAllowed = false;
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull().And.BeOfType<NoContent>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenUserIsTerminated()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _user.TerminatedOnDateTime = DateTime.UtcNow;
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesPasswordResetToken()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);
        _userManager.GeneratePasswordResetTokenAsync(_user).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        _userManager.Received(1).GeneratePasswordResetTokenAsync(_user);
    }

    [Fact]
    public async Task ExecuteAsync_SendsResetPasswordEmail()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);
        _userManager.GeneratePasswordResetTokenAsync(_user).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        _emailService.Received(1).SendResetPasswordEmailAsync("test@test.com", "123456");
    }

    [Fact]
    public async Task ExecuteAsync_AuditsForgotPassword()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);
        _userManager.GeneratePasswordResetTokenAsync(_user).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        _auditService.Received(1).AuditUserActionAsync(1, AuditLogTypeEnum.ForgotPassword);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenAllSuccessful()
    {
        var request = new ForgotPasswordCommand{ Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(_user);
        _userManager.GeneratePasswordResetTokenAsync(_user).Returns("123456");

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull().And.BeOfType<NoContent>();
    }
}
