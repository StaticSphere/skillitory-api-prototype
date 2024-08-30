using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class OtpTypeConfiguration : IEntityTypeConfiguration<OtpType>
{
    public void Configure(EntityTypeBuilder<OtpType> builder)
    {
        builder.ToTable("otp_type", "auth");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
