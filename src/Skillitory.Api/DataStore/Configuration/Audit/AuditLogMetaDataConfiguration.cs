using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Audit;

namespace Skillitory.Api.DataStore.Configuration.Audit;

[ExcludeFromCodeCoverage]
public class AuditLogMetaDataConfiguration : IEntityTypeConfiguration<AuditLogMetaData>
{
    public void Configure(EntityTypeBuilder<AuditLogMetaData> builder)
    {
        builder.ToTable("audit_log_metadata", "audit");
        builder.HasIndex(x => x.AuditLogId);

        builder.Property(x => x.MetaData)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne(x => x.AuditLog)
            .WithMany()
            .HasForeignKey(x => x.AuditLogId)
            .IsRequired();
    }
}
