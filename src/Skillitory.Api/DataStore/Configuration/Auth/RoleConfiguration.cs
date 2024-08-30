using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class RoleConfiguration : IEntityTypeConfiguration<AuthRole>
{
    public void Configure(EntityTypeBuilder<AuthRole> builder)
    {
        builder.ToTable("role", "auth");

        builder.Property(x => x.Description)
            .HasMaxLength(500);
    }
}
