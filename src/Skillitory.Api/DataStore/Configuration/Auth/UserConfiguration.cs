using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class UserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("user", "auth");
        builder.HasIndex(t => t.UserUniqueKey)
            .IsUnique();

        builder.Property(t => t.UserUniqueKey)
            .HasMaxLength(50);

        builder.HasOne(x => x.OtpType)
            .WithMany()
            .HasForeignKey(x => x.OtpTypeId);
    }
}
