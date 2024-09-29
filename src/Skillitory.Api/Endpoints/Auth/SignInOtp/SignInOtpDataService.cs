using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.Endpoints.Auth.SignInOtp;

public class SignInOtpDataService : ISignInOtpDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public SignInOtpDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthUser?> GetUserByUserUniqueKeyAsync(string userUniqueKey, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.UserUniqueKey == userUniqueKey)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
