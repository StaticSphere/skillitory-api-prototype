using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Mbr;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore;

public interface ISkillitoryDbContext
{
    DbSet<AuthUser> Users { get; set; }
    DbSet<AuthRole> Roles { get; set; }
    DbSet<IdentityUserRole<int>> UserRoles { get; set; }
    DbSet<Member> Members { get; set; }
    DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    DbSet<AuditLogType> AuditLogTypes { get; set; }
    DbSet<AuditLog> AuditLogs { get; set; }
    DbSet<AuditLogMetaData> AuditLogMetaDatas { get; set; }
    DbSet<Organization> Organizations { get; set; }
    DbSet<OrganizationChurn> OrganizationChurns { get; set; }
    DbSet<OrganizationChurnCategory> OrganizationChurnCategories { get; set; }
    DbSet<Department> Departments { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
