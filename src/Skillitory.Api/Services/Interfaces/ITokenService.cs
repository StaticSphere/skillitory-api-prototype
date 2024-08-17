using System.Security.Claims;
using Skillitory.Api.Models;

namespace Skillitory.Api.Services.Interfaces;

public interface ITokenService
{
    TokenData GenerateAuthTokens(IEnumerable<Claim> claims);
}
