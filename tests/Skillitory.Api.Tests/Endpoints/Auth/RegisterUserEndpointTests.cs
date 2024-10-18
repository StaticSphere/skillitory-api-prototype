using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skillitory.Api.DataStore;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Mbr;
using Skillitory.Api.Endpoints.Auth.RegisterUser;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class RegisterUserEndpointTests
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IRegisterUserDataService _registerUserDataService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILoggerService<RegisterUserEndpoint> _loggerService;
    private readonly IOptions<SecurityConfiguration> _securityConfiguration;
    private readonly RegisterUserEndpoint _endpoint;

    public RegisterUserEndpointTests()
    {
        var store = Substitute.For<IUserStore<AuthUser>>();
        _userManager =
            Substitute.For<UserManager<AuthUser>>(store, null, null, null, null, null, null, null, null);
        _registerUserDataService = Substitute.For<IRegisterUserDataService>();
        _emailService = Substitute.For<IEmailService>();
        _auditService = Substitute.For<IAuditService>();
        _dateTimeService = Substitute.For<IDateTimeService>();
        _loggerService = Substitute.For<ILoggerService<RegisterUserEndpoint>>();
        _securityConfiguration = Substitute.For<IOptions<SecurityConfiguration>>();

        _securityConfiguration.Value.Returns(new SecurityConfiguration
        {
            Password = new PasswordConfiguration
            {
                DefaultPasswordLifetimeDays = 7
            }
        });

        _endpoint = new RegisterUserEndpoint(
            _userManager,
            _registerUserDataService,
            _emailService,
            _auditService,
            _dateTimeService,
            _loggerService,
            _securityConfiguration);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesCorrectUser()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        AuthUser user = null!;
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => user = x), "password").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).CreateAsync(Arg.Any<AuthUser>(), "password");
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(new
        {
            Email = "email@email.com",
            UserName = "email@email.com",
            LastPasswordChangedDateTime = date,
            PasswordExpirationDateTime = date.AddDays(7),
            Member = new Member
            {
                FirstName = "Test",
                LastName = "User"
            }
        });
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesPasswordHistory()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Any<AuthUser>(), "password").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).UpdateAsync(Arg.Any<AuthUser>());
    }

    [Fact]
    public async Task ExecuteAsync_AddsUserToOrgAdminRole()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Any<AuthUser>(), "password").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).AddToRoleAsync(Arg.Any<AuthUser>(), DataStoreConstants.OrganizationAdministratorRoleName);
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesEmailConfirmationToken()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Any<AuthUser>(), "password").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _userManager.Received(1).GenerateEmailConfirmationTokenAsync(Arg.Any<AuthUser>());
    }

    [Fact]
    public async Task ExecuteAsync_SendsEmailConfirmationEmail()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Any<AuthUser>(), "password").Returns(IdentityResult.Success);
        _userManager.GenerateEmailConfirmationTokenAsync(Arg.Any<AuthUser>()).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        await _emailService.Received(1).SendEmailConfirmationEmailAsync("email@email.com", "123456");
    }

    [Fact]
    public async Task ExecuteAsync_AuditsUserAction()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Success);
        _userManager.GenerateEmailConfirmationTokenAsync(Arg.Any<AuthUser>()).Returns("123456");

        await _endpoint.ExecuteAsync(request, default);

        await _auditService.Received(1).AuditUserActionAsync(1, AuditLogTypeEnum.NewUserRegistered);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNoContent_WhenSuccessful()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password" };
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Success);
        _userManager.GenerateEmailConfirmationTokenAsync(Arg.Any<AuthUser>()).Returns("123456");

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ExecuteAsync_GetsExistingOrganization_WhenOrganizationPassedIn()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password", Organization = "Test Org"};
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Success);

        await _endpoint.ExecuteAsync(request, default);

        await _registerUserDataService.Received(1).GetOrganizationExistsAsync("Test Org");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsConflict_WhenOrganizationExists()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password", Organization = "Test Org"};
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Success);
        _registerUserDataService.GetOrganizationExistsAsync("Test Org").Returns(true);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Conflict<string>>()
            .Which.Value.Should().Be("Organization already exists");
    }

    [Fact]
    public async Task ExecuteAsync_LogsError_WhenCreateUserFails()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password"};
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Failed(new IdentityError { Code = "123", Description = "Oops" }));
        _registerUserDataService.GetOrganizationExistsAsync("Test Org").Returns(true);

        await _endpoint.ExecuteAsync(request, default);

        _loggerService.Received(1).LogError("An error occurred trying to register a new user: {errors}", "Code: 123, Description: Oops");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsConflict_WhenUserNameDuplicate()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password"};
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "Oops" }));
        _registerUserDataService.GetOrganizationExistsAsync("Test Org").Returns(true);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Conflict<string>>()
            .Which.Value.Should().Be("User account already exists");
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsInternalServerError_WhenCreateUserFails()
    {
        var date = DateTimeOffset.UtcNow;
        var request = new RegisterUserCommand { Email = "email@email.com", FirstName = "Test", LastName = "User", Password = "password"};
        _dateTimeService.UtcNow.ReturnsForAnyArgs(date);
        _userManager.CreateAsync(Arg.Do<AuthUser>(x => x.Id = 1), "password").Returns(IdentityResult.Failed(new IdentityError { Code = "123", Description = "Oops" }));
        _registerUserDataService.GetOrganizationExistsAsync("Test Org").Returns(true);

        var result = await _endpoint.ExecuteAsync(request, default);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<StatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(500);
    }
}
