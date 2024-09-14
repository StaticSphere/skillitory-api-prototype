using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth;

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
        var expiredTokens = await _dbContext.UserRefreshTokens
            .Where(x => x.ExpirationDateTime <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _dbContext.UserRefreshTokens.RemoveRange(expiredTokens);

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
}
