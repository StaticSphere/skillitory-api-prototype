using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore.Configuration.Org;

[ExcludeFromCodeCoverage]
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department", "org");
        builder.HasKey(x => x.DepartmentId);
        builder.HasIndex(x => x.DepartmentUniqueKey)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(x => new { x.OrganizationId, x.Name })
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Notes)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.OrganizationId)
            .IsRequired();

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
