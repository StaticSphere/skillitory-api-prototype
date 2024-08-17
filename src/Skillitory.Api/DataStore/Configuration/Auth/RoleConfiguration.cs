using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class RoleConfiguration : IEntityTypeConfiguration<SkillitoryRole>
{
    public void Configure(EntityTypeBuilder<SkillitoryRole> builder)
    {
        builder.ToTable("role", "auth");

        builder.Property(x => x.Description)
            .HasMaxLength(500);
    }
}
