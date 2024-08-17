using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Common.Enumerations;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.DataStore.Entities.Auth;
using Visus.Cuid;

namespace Skillitory.Api.DataStore;

[ExcludeFromCodeCoverage]
public static class ModelBuilderExtensions
{
    public static void SeedData(this ModelBuilder builder)
    {
        builder.Entity<SkillitoryUser>().HasData(
            new SkillitoryUser
            {
                Id = 1,
                UserUniqueKey = new Cuid2().ToString(),
                Email = "system_user@skillitory.com",
                NormalizedEmail = "SYSTEM_USER@SKILLITORY.COM",
                FirstName = "SYSTEM",
                LastName = "USER",
                UserName = "system_user@skillitory.com",
                NormalizedUserName = "SYSTEM_USER@SKILLITORY.COM",
                IsSignInAllowed = false,
                IsSystemUser = true,
                CreatedBy = 1,
                CreatedDateTime = DateTimeOffset.UtcNow,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                SecurityStamp = "NEVER_GOING_TO_SIGN_IN"
            }
        );

        builder.Entity<SkillitoryRole>().HasData(
            new SkillitoryRole
            {
                Id = 1,
                Name = DataStoreConstants.SkillitoryAdministratorRoleName,
                NormalizedName = DataStoreConstants.SkillitoryAdministratorRoleName.ToUpper(),
                Description =
                    "Users in this role can read and write all Skillitory resources, including customer data.",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsApplicationAdministratorRole = true
            },
            new SkillitoryRole
            {
                Id = 2,
                Name = DataStoreConstants.SkillitoryViewerRoleName,
                NormalizedName = DataStoreConstants.SkillitoryViewerRoleName.ToUpper(),
                Description = "Users in this role can read all Skillitory resources, including customer data.",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsApplicationAdministratorRole = true
            },
            new SkillitoryRole
            {
                Id = 3,
                Name = DataStoreConstants.OrganizationAdministratorRoleName,
                NormalizedName = DataStoreConstants.OrganizationAdministratorRoleName.ToUpper(),
                Description = "Users in this role can administrate the organizations that they're associated with.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new SkillitoryRole
            {
                Id = 4,
                Name = DataStoreConstants.OrganizationViewerRoleName,
                NormalizedName = DataStoreConstants.OrganizationViewerRoleName.ToUpper(),
                Description =
                    "Users in this role can view the details and users of the organizations that they're associated with.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new SkillitoryRole
            {
                Id = 5,
                Name = DataStoreConstants.UserRoleName,
                NormalizedName = DataStoreConstants.UserRoleName.ToUpper(),
                Description =
                    "Users in this role are standard users that can manage their own profile, skills, goals, etc.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        // builder.Entity<Organization>().HasData(
        //     new Organization
        //     {
        //         OrganizationId = 1,
        //         Name = "StaticSphere",
        //         Description = "The organization that owns and developed Skillitory.",
        //         OrganizationUniqueKey = Guid.NewGuid(),
        //         IsSystemOrganization = true,
        //         CreatedBy = 1,
        //         CreatedOn = DateTime.UtcNow
        //     }
        // );
        //
        // builder.Entity<OrganizationChurnCategory>().HasData(
        //     new OrganizationChurnCategory
        //     {
        //         OrganizationChurnCategoryId = 1,
        //         Name = "Unhappy with Application",
        //         Description = "The user has churned because they were unhappy with some element of the application."
        //     },
        //     new OrganizationChurnCategory
        //     {
        //         OrganizationChurnCategoryId = 2,
        //         Name = "Unhappy with Service",
        //         Description = "The user has churned because they were unhappy with service."
        //     },
        //     new OrganizationChurnCategory
        //     {
        //         OrganizationChurnCategoryId = 3,
        //         Name = "No Longer Price Effective",
        //         Description = "The user has churned because they no longer feel that the product is cost effective."
        //     },
        //     new OrganizationChurnCategory
        //     {
        //         OrganizationChurnCategoryId = 4,
        //         Name = "Other",
        //         Description =
        //             "The user has churned for a reason other than that provided by the other churn options."
        //     }
        // );

        builder.Entity<OtpType>().HasData(
            Enum.GetValues<OtpTypeEnum>()
                .Select(x => new OtpType { Id = x, Name = x.ToString() })
                .ToArray());

        builder.Entity<AuditLogType>().HasData(
            Enum.GetValues<AuditLogTypeEnum>()
                .Select(x => new AuditLogType { Id = x, Name = x.ToString() })
                .ToArray());
        //
        // builder.Entity<StoredFileType>().HasData(
        //     Enum.GetValues<StoredFileTypeEnum>()
        //         .Select(x => new StoredFileType { Id = x, Name = x.ToString() })
        //         .ToArray());
    }
}
