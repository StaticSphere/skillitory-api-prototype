using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;

namespace Skillitory.Api.Endpoints.Auth.RegisterUser;

[ExcludeFromCodeCoverage]
public class RegisterUserDataService : IRegisterUserDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public RegisterUserDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> GetOrganizationExistsAsync(string organization, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations.AnyAsync(x => x.Name.ToLower() == organization.ToLower(), cancellationToken);
    }
}
