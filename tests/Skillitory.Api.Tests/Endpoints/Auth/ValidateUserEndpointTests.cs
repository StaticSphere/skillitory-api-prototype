using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Endpoints.Auth.ValidateUser;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class ValidateUserEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IValidateUserDataService _validateUserDataService;
    private readonly IAuditService _auditService;
    private readonly ValidateUserEndpoint _endpoint;

    public ValidateUserEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _validateUserDataService = Substitute.For<IValidateUserDataService>();
        _auditService = Substitute.For<IAuditService>();

        _endpoint = new ValidateUserEndpoint(
            _userManager,
            _validateUserDataService,
            _auditService);
    }

    [Fact]
    public async Task ExecuteAsync_FindsUserByEmail()
    {
        var request = new ValidateUserCommand { Email = "test@test.com" };

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).FindByEmailAsync("test@test.com");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNotFound_WhenUserNotFound()
    {
        var request = new ValidateUserCommand { Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns((AuthUser)null!);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ExecuteAsync_ConfirmsEmail()
    {
        var request = new ValidateUserCommand { Email = "test@test.com", Token = "123456" };
        var user = new AuthUser { Id = 1, Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ConfirmEmailAsync(user, "123456").Returns(IdentityResult.Failed());

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).ConfirmEmailAsync(user, "123456");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnprocessableEntity_WhenConfirmEmailFails()
    {
        var request = new ValidateUserCommand { Email = "test@test.com", Token = "123456" };
        var user = new AuthUser { Id = 1, Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ConfirmEmailAsync(user, "123456").Returns(IdentityResult.Failed());

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnprocessableEntity>();
    }

    [Fact]
    public async Task ExecuteAsync_EnablesSignIn()
    {
        var request = new ValidateUserCommand { Email = "test@test.com", Token = "123456" };
        var user = new AuthUser { Id = 1, Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ConfirmEmailAsync(user, "123456").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _validateUserDataService.Received(1).EnableSignInAsync(1);
    }

    [Fact]
    public async Task ExecuteAsync_AuditsEmailVerified()
    {
        var request = new ValidateUserCommand { Email = "test@test.com", Token = "123456" };
        var user = new AuthUser { Id = 1, Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ConfirmEmailAsync(user, "123456").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _auditService.Received(1).AuditUserActionAsync(1, AuditLogTypeEnum.NewUserEmailVerified);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_OnSuccess()
    {
        var request = new ValidateUserCommand { Email = "test@test.com", Token = "123456" };
        var user = new AuthUser { Id = 1, Email = "test@test.com" };
        _userManager.FindByEmailAsync("test@test.com").Returns(user);
        _userManager.ConfirmEmailAsync(user, "123456").Returns(IdentityResult.Success);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NoContent>();
    }
}
