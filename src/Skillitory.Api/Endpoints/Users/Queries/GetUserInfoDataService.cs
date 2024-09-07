using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore;

namespace Skillitory.Api.Endpoints.Users.Queries;

public class GetUserInfoDataService : IGetUserInfoDataService
{
    private readonly ISkillitoryDbContext _dbContext;

    public GetUserInfoDataService(ISkillitoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetUserInfoResponse?> GetUserInfoAsync(string userUniqueKey, CancellationToken cancellationToken = default)
    {
        return await (from u in _dbContext.Users
                join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                join r in _dbContext.Roles on ur.RoleId equals r.Id
                join m in _dbContext.Members on u.Id equals m.UserId
                join o in _dbContext.Organizations on m.OrganizationId equals o.OrganizationId into lo
                    from o in lo.DefaultIfEmpty()
                join d in _dbContext.Departments on m.DepartmentId equals d.DepartmentId into ld
                    from d in ld.DefaultIfEmpty()
                where u.UserUniqueKey == userUniqueKey
                select new GetUserInfoResponse
                {
                    UserUniqueKey = u.UserUniqueKey,
                    OrganizationUniqueKey = o.OrganizationUniqueKey,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = u.Email!,
                    Organization = o.Name,
                    Department = d.Name,
                    Title = m.Title,
                    AvatarVersionKey = null,
                    Role = r.Name!,
                }).FirstOrDefaultAsync(cancellationToken);
    }
}
