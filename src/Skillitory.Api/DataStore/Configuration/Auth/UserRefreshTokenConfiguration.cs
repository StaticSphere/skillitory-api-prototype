using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("user_refresh_token", "auth");
        builder.HasKey(x => x.UniqueKey);

        builder.HasIndex(x => new { x.UserId, x.Jti })
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.Token })
            .IsUnique();

        builder.Property(x => x.UniqueKey)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Jti)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Token)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}
