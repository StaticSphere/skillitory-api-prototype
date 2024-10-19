using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.ResetPassword;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class ResetPasswordEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IAuditService _auditService;
    private readonly ResetPasswordEndpoint _endpoint;

    public ResetPasswordEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _auditService = Substitute.For<IAuditService>();
        _endpoint = new ResetPasswordEndpoint(_userManager, _auditService);
    }

    [Fact]
    public async Task ExecuteAsync_TrysToFindUserByEmail()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com" };

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).FindByEmailAsync("test@test.com");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserNotFound()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns((AuthUser?)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserNotAllowedSignIn()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = false };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserIsTerminated()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true, TerminatedOnDateTime = DateTimeOffset.UtcNow };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ExecuteAsync_ResetsPassword()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).ResetPasswordAsync(user, "123456", "789012");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsGone_WhenResetTokenIsInvalid()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Failed(new IdentityError { Code = "InvalidToken" }));

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<StatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(410);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnprocessable_WhenPasswordIsInvalid()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Failed(new IdentityError { Code = "PasswordInvalid" }));

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnprocessableEntity>();
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsServerError_WhenResetPasswordFailsGenerally()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Failed(new IdentityError { Code = "Something went horribly wrong..." }));

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<StatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ExecuteAsync_ResetsAccessFailedCount_WhenResetSuccessful()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).ResetAccessFailedCountAsync(user);
    }

    [Fact]
    public async Task ExecuteAsync_AuditsPasswordReset_WhenResetSuccessful()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _auditService.AuditUserActionAsync(1, AuditLogTypeEnum.ResetPassword);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenResetSuccessful()

    {
        var request = new ResetPasswordCommand { Email = "test@test.com", Token = "123456", Password = "789012" };
        var user = new AuthUser { Id = 1, Email = "test@test.com", IsSignInAllowed = true };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ResetPasswordAsync(user, "123456", "789012").Returns(IdentityResult.Success);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NoContent>();
    }
}
