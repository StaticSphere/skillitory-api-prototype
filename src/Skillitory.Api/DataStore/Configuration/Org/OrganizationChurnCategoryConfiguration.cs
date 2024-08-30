using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Org;

namespace Skillitory.Api.DataStore.Configuration.Org;

[ExcludeFromCodeCoverage]
public class OrganizationChurnCategoryConfiguration : IEntityTypeConfiguration<OrganizationChurnCategory>
{
    public void Configure(EntityTypeBuilder<OrganizationChurnCategory> builder)
    {
        builder.ToTable("organization_churn_category", "org");
        builder.HasKey(x => x.OrganizationChurnCategoryId);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);
    }
}
