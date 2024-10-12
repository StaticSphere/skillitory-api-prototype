using FluentAssertions;
using Skillitory.Api.Endpoints.Auth.RefreshTokens;
using Skillitory.Api.Models;

namespace Skillitory.Api.Tests.Endpoints.Auth;

public class RefreshTokensCommandResponseTests
{
    [Fact]
    public void MappingFromTokenData_Succeeds()
    {
        var accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(5);
        var refreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(7);

        var tokenData = new TokenData
        {
            AccessToken = "Test_Access_Token",
            RefreshToken = "Test_Refresh_Token",
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };

        var response = (RefreshTokensCommandResponse)tokenData;

        response.Should().BeEquivalentTo(tokenData);
    }
}
