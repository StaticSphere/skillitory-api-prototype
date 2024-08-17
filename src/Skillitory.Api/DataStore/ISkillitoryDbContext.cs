using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore;

public interface ISkillitoryDbContext
{
    DbSet<SkillitoryUser> Users { get; set; }
    DbSet<SkillitoryRole> Roles { get; set; }
    DbSet<IdentityUserRole<int>> UserRoles { get; set; }
    DbSet<AuditLogType> AuditLogTypes { get; set; }
    DbSet<AuditLog> AuditLogs { get; set; }
    DbSet<AuditLogMetaData> AuditLogMetaDatas { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
