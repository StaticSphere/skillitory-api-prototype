using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth;

[ExcludeFromCodeCoverage]
public class UserRefreshTokenDataService : IUserRefreshTokenDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public UserRefreshTokenDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveUserRefreshTokenAsync(int userId, Guid jti, string refreshToken, DateTimeOffset refreshTokenExpiration,
        CancellationToken cancellationToken = default)
    {
        await PurgeUserExpiredRefreshTokensAsync(userId, cancellationToken);

        _dbContext.UserRefreshTokens.Add(new UserRefreshToken
        {
            UserId = userId,
            Jti = jti.ToString(),
            Token = refreshToken,
            ExpirationDateTime = refreshTokenExpiration,
            CreatedDateTime = DateTime.UtcNow,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userRefreshToken = await _dbContext.UserRefreshTokens
            .AsTracking()
            .Where(x => x.Token == refreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (userRefreshToken is not null)
        {
            await PurgeUserExpiredRefreshTokensAsync(userRefreshToken.UserId, cancellationToken);

            _dbContext.UserRefreshTokens.Remove(userRefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task PurgeUserExpiredRefreshTokensAsync(int userId, CancellationToken cancellationToken)
    {
        var expiredTokens = await _dbContext.UserRefreshTokens
            .Where(x => x.UserId == userId && x.ExpirationDateTime <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _dbContext.UserRefreshTokens.RemoveRange(expiredTokens);
    }
}
