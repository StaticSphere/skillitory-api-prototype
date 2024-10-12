using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Skillitory.Api.DataStore;
using Skillitory.Api.Exceptions;
using Skillitory.Api.Services;

namespace Skillitory.Api.Tests.Services;

public class PrincipalServiceTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PrincipalService _principalService;

    public PrincipalServiceTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _principalService = new PrincipalService(_httpContextAccessor);
    }

    [Fact]
    public void UserUniqueKeyReturnsWhenPrincipalFound()
    {
        _httpContextAccessor.HttpContext!.User.Returns(new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, "test_key")])));

        _principalService.UserUniqueKey.Should().Be("test_key");
    }

    [Fact]
    public void UserUniqueKeyThrowsWhenPrincipalNotFound()
    {
        _httpContextAccessor.HttpContext!.User.Returns((ClaimsPrincipal)null!);

        var action = () => _principalService.UserUniqueKey;

        action.Should().Throw<MissingClaimsPrincipalException>();
    }

    [Fact]
    public void IsUserInAnyRoleReturnsTrueWhenUserInRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        var isUserInAnyRole = _principalService.IsUserInAnyRole(
            DataStoreConstants.OrganizationAdministratorRoleName);

        isUserInAnyRole.Should().BeTrue();
    }

    [Fact]
    public void IsUserInAnyRoleReturnsFalseWhenUserNotInRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        var isUserInAnyRole = _principalService.IsUserInAnyRole(
            DataStoreConstants.UserRoleName);

        isUserInAnyRole.Should().BeFalse();
    }

    [Fact]
    public void IsSkillitoryAdministratorReturnsTrueWhenUserInSkillitoryAdministratorRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsSkillitoryAdministrator.Should().BeTrue();
    }

    [Fact]
    public void IsSkillitoryAdministratorReturnsFalseWhenUserNotInSkillitoryAdministratorRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.UserRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsSkillitoryAdministrator.Should().BeFalse();
    }

    [Fact]
    public void IsSkillitoryViewerReturnsTrueWhenUserInSkillitoryViewerRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryViewerRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsSkillitoryViewer.Should().BeTrue();
    }

    [Fact]
    public void IsSkillitoryViewerReturnsFalseWhenUserNotInSkillitoryViewerRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.UserRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsSkillitoryViewer.Should().BeFalse();
    }

    [Fact]
    public void IsOrganizationAdministratorReturnsTrueWhenUserInOrganizationAdministratorRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsOrganizationAdministrator.Should().BeTrue();
    }

    [Fact]
    public void IsOrganizationAdministratorReturnsFalseWhenUserNotInOrganizationAdministratorRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.UserRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsOrganizationAdministrator.Should().BeFalse();
    }

    [Fact]
    public void IsOrganizationViewerReturnsTrueWhenUserInOrganizationViewerRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationViewerRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsOrganizationViewer.Should().BeTrue();
    }

    [Fact]
    public void IsOrganizationViewerReturnsFalseWhenUserNotInOrganizationViewerRole()
    {
        var userPrincipal = new ClaimsPrincipal(
        [
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "test_key"),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.SkillitoryViewerRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.OrganizationAdministratorRoleName),
                        new Claim(ClaimTypes.Role, DataStoreConstants.UserRoleName)
            ])
        ]);
        _httpContextAccessor.HttpContext!.User.Returns(userPrincipal);

        _principalService.IsOrganizationViewer.Should().BeFalse();
    }
}
