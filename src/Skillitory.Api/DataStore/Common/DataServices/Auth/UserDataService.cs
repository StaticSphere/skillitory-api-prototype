using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.DataServices.Auth.Interfaces;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Common.DataServices.Auth;

public class UserDataService : IUserDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public UserDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
