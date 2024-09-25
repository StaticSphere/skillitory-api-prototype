using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Com;

namespace Skillitory.Api.DataStore.Configuration.Com;

public class CommunicationTemplateConfiguration : IEntityTypeConfiguration<CommunicationTemplate>
{
    public void Configure(EntityTypeBuilder<CommunicationTemplate> builder)
    {
        builder.ToTable("communication_template", "com");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Template)
            .IsRequired();

        builder.HasOne(x => x.CommunicationTemplateType)
            .WithMany()
            .HasForeignKey(x => x.CommunicationTemplateTypeId)
            .IsRequired();
    }
}
