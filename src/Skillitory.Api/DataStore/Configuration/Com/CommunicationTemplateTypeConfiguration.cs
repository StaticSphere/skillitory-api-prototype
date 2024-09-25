using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Com;

namespace Skillitory.Api.DataStore.Configuration.Com;

public class CommunicationTemplateTypeConfiguration : IEntityTypeConfiguration<CommunicationTemplateType>
{
    public void Configure(EntityTypeBuilder<CommunicationTemplateType> builder)
    {
        builder.ToTable("communication_template_type", "com");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}
