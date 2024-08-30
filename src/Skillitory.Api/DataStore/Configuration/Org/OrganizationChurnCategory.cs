using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore.Configuration.Org;

[ExcludeFromCodeCoverage]
public class OrganizationChurnConfiguration : IEntityTypeConfiguration<OrganizationChurn>
{
    public void Configure(EntityTypeBuilder<OrganizationChurn> builder)
    {
        builder.ToTable("organization_churn", "org");
        builder.HasKey(x => x.OrganizationChurnId);

        builder.Property(x => x.Details)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.OrganizationChurns)
            .HasForeignKey(x => x.OrganizationId)
            .IsRequired();

        builder.HasOne(x => x.OrganizationChurnCategory)
            .WithMany()
            .HasForeignKey(x => x.OrganizationChurnCategoryId)
            .IsRequired();
    }
}
