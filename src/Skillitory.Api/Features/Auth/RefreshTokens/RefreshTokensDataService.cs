using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.Features.Auth.RefreshTokens;

public class RefreshTokensDataService : IRefreshTokensDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public RefreshTokensDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserRefreshToken?> GetUserRefreshTokenAsync(string email, string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsTracking()
            .Include(x => x.RefreshTokens)
            .Where(x => x.UserName == email)
            .Select(x => x.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateUserRefreshTokenAsync(UserRefreshToken userRefreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.UserRefreshTokens.Attach(userRefreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
