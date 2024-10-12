using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.Models;
using Skillitory.Api.Models.Configuration;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Services;

[ExcludeFromCodeCoverage(Justification = "Not really sure how to make a useful test here, but will revisit later if it makes sense to do so.")]
public class TokenService : ITokenService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IDateTimeService _dateTimeService;
    private readonly SecurityConfiguration _securityConfiguration;

    public TokenService(
        UserManager<AuthUser> userManager,
        IDateTimeService dateTimeService,
        IOptions<SecurityConfiguration> securityConfiguration)
    {
        _userManager = userManager;
        _dateTimeService = dateTimeService;
        _securityConfiguration = securityConfiguration.Value;
    }

    public async Task<TokenData> GenerateAuthTokensAsync(AuthUser user, Guid jti, CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserUniqueKey),
            new(JwtRegisteredClaimNames.Jti, jti.ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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

    public ClaimsPrincipal? GetClaimsPrincipalFromAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = _securityConfiguration.Jwt.ValidIssuer,
            ValidAudience = _securityConfiguration.Jwt.ValidAudience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityConfiguration.Jwt.Secret)),
            ValidateLifetime = false,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}
