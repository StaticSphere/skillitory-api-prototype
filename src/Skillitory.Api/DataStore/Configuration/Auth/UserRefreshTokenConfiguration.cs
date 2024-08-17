using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("user_refresh_token", "auth");
        builder.HasKey(x => new { x.UserId, x.Token });

        builder.Property(x => x.Token)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}
