using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Skillitory.Api.Models;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

[ExcludeFromCodeCoverage(Justification = "Not really sure how to make a useful test here, but will revisit later if it makes sense to do so.")]
public class TokenService : ITokenService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly SecurityConfiguration _securityConfiguration;

    public TokenService(
        IDateTimeService dateTimeService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _dateTimeService = dateTimeService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public TokenData GenerateAuthTokens(IEnumerable<Claim> claims)
    {
        // Access Token
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityConfiguration.Jwt.Secret));
        var accessTokenExpiration = _dateTimeService.UtcNow.AddMinutes(_securityConfiguration.Jwt.TokenValidityMinutes);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _securityConfiguration.Jwt.ValidIssuer,
            audience: _securityConfiguration.Jwt.ValidAudience,
            expires: accessTokenExpiration,
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        // Refresh Token
        var refreshTokenBytes = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(refreshTokenBytes);
        var refreshToken = Convert.ToBase64String(refreshTokenBytes);
        var refreshTokenExpiration =
            _dateTimeService.UtcNow.AddDays(_securityConfiguration.Jwt.RefreshTokenValidityDays);

        return new TokenData
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }
}
