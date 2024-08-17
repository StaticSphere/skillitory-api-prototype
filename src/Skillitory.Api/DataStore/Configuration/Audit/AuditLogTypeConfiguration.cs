using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Audit;

namespace Skillitory.Api.DataStore.Configuration.Audit;

[ExcludeFromCodeCoverage]
public class AuditLogTypeConfiguration : IEntityTypeConfiguration<AuditLogType>
{
    public void Configure(EntityTypeBuilder<AuditLogType> builder)
    {
        builder.ToTable("audit_log_type", "audit");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
