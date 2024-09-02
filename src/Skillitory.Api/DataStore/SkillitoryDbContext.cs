using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Mbr;
using Skillitory.Api.DataStore.Entities.Org;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.DataStore;

[ExcludeFromCodeCoverage]
public class SkillitoryDbContext : IdentityDbContext<AuthUser, AuthRole, int>, ISkillitoryDbContext
{
    private readonly IConfiguration _configuration;
    private readonly IDateTimeService _dateTimeService;
    private readonly IPrincipalService _principalService;

    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = null!;
    public DbSet<AuditLogType> AuditLogTypes { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<AuditLogMetaData> AuditLogMetaDatas { get; set; } = null!;
    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<OrganizationChurn> OrganizationChurns { get; set; } = null!;
    public DbSet<OrganizationChurnCategory> OrganizationChurnCategories { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;

    public SkillitoryDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IPrincipalService principalService,
        IDateTimeService dateTimeService)
        : base(options)
    {
        _configuration = configuration;
        _principalService = principalService;
        _dateTimeService = dateTimeService;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entity in ChangeTracker.Entries<IAuditableEntity>()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var userId = 1;
            if (_principalService.IsAuthenticated)
                userId = await Users.Where(x => x.UserUniqueKey == _principalService.UserUniqueKey).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);

            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreatedBy =  userId;
                entity.Entity.CreatedDateTime = _dateTimeService.UtcNow;
            }
            else if (_principalService.IsAuthenticated)
            {
                entity.Entity.UpdatedBy = userId;
                entity.Entity.UpdatedDateTime = _dateTimeService.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseNpgsql(
                _configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.SeedData();
    }
}
