using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillitory.Api.DataStore.Entities.Auth;

namespace Skillitory.Api.DataStore.Configuration.Auth;

[ExcludeFromCodeCoverage]
public class UserConfiguration : IEntityTypeConfiguration<SkillitoryUser>
{
    public void Configure(EntityTypeBuilder<SkillitoryUser> builder)
    {
        builder.ToTable("user", "auth");
        builder.HasIndex(t => t.UserUniqueKey)
            .IsUnique();

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

        // builder.HasOne(x => x.Organization)
        //     .WithMany(x => x.Users)
        //     .HasForeignKey(x => x.OrganizationId);
        //
        // builder.HasOne(x => x.Department)
        //     .WithMany()
        //     .HasForeignKey(x => x.DepartmentId);

        builder.HasOne(x => x.Supervisor)
            .WithMany()
            .HasForeignKey(x => x.SupervisorId);

        // builder.HasOne(x => x.AvatarStoredFile)
        //     .WithMany()
        //     .HasForeignKey(x => x.AvatarStoredFileId);
    }
}
