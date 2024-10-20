using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;

namespace Skillitory.Api.Endpoints.Auth.SignOutAllDevices;

[ExcludeFromCodeCoverage]
public class SignOutAllDevicesDataService : ISignOutAllDevicesDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public SignOutAllDevicesDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteAllUserRefreshTokensAsync(string userUniqueKey, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsTracking()
            .Include(x => x.RefreshTokens)
            .Where(x => x.UserUniqueKey == userUniqueKey)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return;

        foreach(var refreshToken in user.RefreshTokens)
            _dbContext.UserRefreshTokens.Remove(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
