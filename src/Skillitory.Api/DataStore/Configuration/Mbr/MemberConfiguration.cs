using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Mbr;

namespace Skillitory.Api.DataStore.Configuration.Mbr;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("member", "mbr");
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(t => t.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Biography)
            .HasMaxLength(4000);

        builder.Property(t => t.Education)
            .HasMaxLength(1000);

        builder.Property(t => t.ExternalId)
            .HasMaxLength(50);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Member)
            .HasForeignKey<Member>(x => x.UserId);

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationId);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId);

        builder.HasOne(x => x.Supervisor)
            .WithMany()
            .HasForeignKey(x => x.SupervisorId);

        // builder.HasOne(x => x.AvatarStoredFile)
        //     .WithMany()
        //     .HasForeignKey(x => x.AvatarStoredFileId);
    }
}
