using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;

namespace Skillitory.Api.Features.Auth.SignOut;

public class SignOutDataService : ISignOutDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public SignOutDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteUserRefreshTokenAsync(string userUniqueKey, string refreshToken, CancellationToken cancellationToken = default)
    {
        var dbRefreshToken = await _dbContext.Users
            .AsTracking()
            .Include(x => x.RefreshTokens)
            .Where(x => x.UserUniqueKey == userUniqueKey)
            .Select(x => x.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync(cancellationToken);

        if (dbRefreshToken is not null)
        {
            _dbContext.UserRefreshTokens.Remove(dbRefreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
