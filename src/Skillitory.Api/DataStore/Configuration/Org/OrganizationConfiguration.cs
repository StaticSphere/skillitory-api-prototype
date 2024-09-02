using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore.Configuration.Org;

[ExcludeFromCodeCoverage]
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organization", "org");
        builder.HasKey(x => x.OrganizationId);
        builder.HasIndex(x => x.OrganizationUniqueKey)
            .IsUnique();

        builder.Property(x => x.OrganizationUniqueKey)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Notes)
            .HasMaxLength(4000);

        builder.Property(x => x.ExternalIdName)
            .HasMaxLength(100);

        // builder.HasOne(x => x.LogoStoredFile)
        //     .WithMany()
        //     .HasForeignKey(x => x.LogoStoredFileId);

        builder.HasOne(x => x.CreationUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .IsRequired();

        builder.HasOne(x => x.UpdatingUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy);
    }
}
