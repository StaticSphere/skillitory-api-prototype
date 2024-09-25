using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skillitory.Api.DataStore.Entities.Audit;
using Skillitory.Api.DataStore.Entities.Audit.Enumerations;
using Skillitory.Api.DataStore.Entities.Auth;
using Skillitory.Api.DataStore.Entities.Auth.Enumerations;
using Skillitory.Api.DataStore.Entities.Com;
using Skillitory.Api.DataStore.Entities.Com.Enumerations;
using Skillitory.Api.DataStore.Entities.Org;
using Visus.Cuid;

namespace Skillitory.Api.DataStore;

[ExcludeFromCodeCoverage]
public static class ModelBuilderExtensions
{
    public static void SeedData(this ModelBuilder builder)
    {
        builder.Entity<AuthUser>().HasData(
            new AuthUser
            {
                Id = 1,
                UserUniqueKey = new Cuid2().ToString(),
                Email = "system_user@skillitory.com",
                NormalizedEmail = "SYSTEM_USER@SKILLITORY.COM",
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

        builder.Entity<AuthRole>().HasData(
            new AuthRole
            {
                Id = 1,
                Name = DataStoreConstants.SkillitoryAdministratorRoleName,
                NormalizedName = DataStoreConstants.SkillitoryAdministratorRoleName.ToUpper(),
                Description =
                    "Users in this role can read and write all Skillitory resources, including customer data.",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsApplicationAdministratorRole = true
            },
            new AuthRole
            {
                Id = 2,
                Name = DataStoreConstants.SkillitoryViewerRoleName,
                NormalizedName = DataStoreConstants.SkillitoryViewerRoleName.ToUpper(),
                Description = "Users in this role can read all Skillitory resources, including customer data.",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsApplicationAdministratorRole = true
            },
            new AuthRole
            {
                Id = 3,
                Name = DataStoreConstants.OrganizationAdministratorRoleName,
                NormalizedName = DataStoreConstants.OrganizationAdministratorRoleName.ToUpper(),
                Description = "Users in this role can administrate the organizations that they're associated with.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AuthRole
            {
                Id = 4,
                Name = DataStoreConstants.OrganizationViewerRoleName,
                NormalizedName = DataStoreConstants.OrganizationViewerRoleName.ToUpper(),
                Description =
                    "Users in this role can view the details and users of the organizations that they're associated with.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new AuthRole
            {
                Id = 5,
                Name = DataStoreConstants.UserRoleName,
                NormalizedName = DataStoreConstants.UserRoleName.ToUpper(),
                Description =
                    "Users in this role are standard users that can manage their own profile, skills, goals, etc.",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        builder.Entity<Organization>().HasData(
            new Organization
            {
                OrganizationId = 1,
                Name = "StaticSphere",
                Description = "The organization that owns and developed Skillitory.",
                OrganizationUniqueKey = new Cuid2().ToString(),
                IsSystemOrganization = true,
                CreatedBy = 1,
                CreatedDateTime = DateTime.UtcNow
            }
        );

        builder.Entity<OrganizationChurnCategory>().HasData(
            new OrganizationChurnCategory
            {
                OrganizationChurnCategoryId = 1,
                Name = "Unhappy with Application",
                Description = "The user has churned because they were unhappy with some element of the application."
            },
            new OrganizationChurnCategory
            {
                OrganizationChurnCategoryId = 2,
                Name = "Unhappy with Service",
                Description = "The user has churned because they were unhappy with service."
            },
            new OrganizationChurnCategory
            {
                OrganizationChurnCategoryId = 3,
                Name = "No Longer Price Effective",
                Description = "The user has churned because they no longer feel that the product is cost effective."
            },
            new OrganizationChurnCategory
            {
                OrganizationChurnCategoryId = 4,
                Name = "Other",
                Description =
                    "The user has churned for a reason other than that provided by the other churn options."
            }
        );

        builder.Entity<CommunicationTemplate>().HasData(
            new CommunicationTemplate
            {
                Id = 1,
                CommunicationTemplateTypeId = CommunicationTemplateTypeEnum.Email,
                Name = "ValidateSkillitoryAccount",
                CreatedBy = 1,
                CreatedDateTime = DateTime.UtcNow,
                Template = @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Skillitory Communication</title>
    <style>
      * {
        margin: 0;
        padding: 0;
        font-family: Verdana, Geneva, Tahoma, sans-serif;
      }

      .container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100dvh;
        background-color: gray;
      }

      .card {
        max-width: 30rem;
        padding: 2rem;
        border: solid 1px darkgray;
        border-radius: 1rem;
        background-color: lightgray;
        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);
      }

      .card__content > p {
        margin: 1rem auto;
      }
    </style>
  </head>
  <body class=""container"">
    <main class=""card"" role=""main"">
      <section class=""card__content"">
        <p>
          Your user account has been created in Skillitory. Please click the
          following link to validate your email address:
        </p>
        <p>
          <a href=""{{ callbackUrl }}"">Verify Email</a>
        </p>
      </section>
    </main>
  </body>
</html>"
            },
            new CommunicationTemplate
            {
                Id = 2,
                CommunicationTemplateTypeId = CommunicationTemplateTypeEnum.Email,
                Name = "ForgotPassword",
                CreatedBy = 1,
                CreatedDateTime = DateTime.UtcNow,
                Template = @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Skillitory Communication</title>
    <style>
      * {
        margin: 0;
        padding: 0;
        font-family: Verdana, Geneva, Tahoma, sans-serif;
      }

      .container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100dvh;
        background-color: gray;
      }

      .card {
        max-width: 30rem;
        padding: 2rem;
        border: solid 1px darkgray;
        border-radius: 1rem;
        background-color: lightgray;
        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);
      }

      .card__content > p {
        margin: 1rem auto;
      }
    </style>
  </head>
  <body class=""container"">
    <main class=""card"" role=""main"">
      <section class=""card__content"">
        <p>
          A request has been sent to set or reset your Skillitory password.
          Please click the link below, which will take you to the Skillitory
          reset password screen.
        </p>
        <p>
          Please note that your new password must abide by the following rules:
        </p>
        <ul>
          <li>Must be at least 8 characters</li>
          <li>At least 1 uppercase letter</li>
          <li>At least 1 lowercase letter</li>
          <li>At least 1 number</li>
          <li>At least 1 symbol</li>
        </ul>
        <p>
          <strong
            ><a href=""{{ callbackUrl }}"" target=""_blank"">
              Please click here to go to the reset password screen.</a
            >
          </strong>
          <br />
        </p>
      </section>
    </main>
  </body>
</html>"
            },
            new CommunicationTemplate
            {
                Id = 3,
                CommunicationTemplateTypeId = CommunicationTemplateTypeEnum.Email,
                Name = "SignInOtp",
                CreatedBy = 1,
                CreatedDateTime = DateTime.UtcNow,
                Template = @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Skillitory Communication</title>
    <style>
      * {
        margin: 0;
        padding: 0;
        font-family: Verdana, Geneva, Tahoma, sans-serif;
      }

      .container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100dvh;
        background-color: gray;
      }

      .card {
        max-width: 30rem;
        padding: 2rem;
        border: solid 1px darkgray;
        border-radius: 1rem;
        background-color: lightgray;
        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);
      }

      .card__content > p {
        margin: 1rem auto;
      }
    </style>
  </head>
  <body class=""container"">
    <main class=""card"" role=""main"">
      <section class=""card__content"">
        <p>
          Please enter the following one time password in Skillitory to complete
          your sign in!
        </p>
        <h1>{{ otp }}</h1>
      </section>
    </main>
  </body>
</html>"
            }
        );

        builder.Entity<OtpType>().HasData(
            Enum.GetValues<OtpTypeEnum>()
                .Select(x => new OtpType { Id = x, Name = x.ToString() })
                .ToArray());

        builder.Entity<AuditLogType>().HasData(
            Enum.GetValues<AuditLogTypeEnum>()
                .Select(x => new AuditLogType { Id = x, Name = x.ToString() })
                .ToArray());

        builder.Entity<CommunicationTemplateType>().HasData(
            Enum.GetValues<CommunicationTemplateTypeEnum>()
                .Select(x => new CommunicationTemplateType{ Id = x, Name = x.ToString() })
                .ToArray());
        //
        // builder.Entity<StoredFileType>().HasData(
        //     Enum.GetValues<StoredFileTypeEnum>()
        //         .Select(x => new StoredFileType { Id = x, Name = x.ToString() })
        //         .ToArray());
    }
}
