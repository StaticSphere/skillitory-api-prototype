using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;

namespace Skillitory.Api.Endpoints.Auth.ValidateUser;

[ExcludeFromCodeCoverage]
public class ValidateUserDataService : IValidateUserDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public ValidateUserDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnableSignInAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsTracking()
            .Where(u => u.Id == userId)
            .FirstAsync(cancellationToken);

        user.IsSignInAllowed = true;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
