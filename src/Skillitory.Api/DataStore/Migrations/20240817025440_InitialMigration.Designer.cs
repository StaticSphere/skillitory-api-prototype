﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Skillitory.Api.DataStore;

#nullable disable

namespace Skillitory.Api.DataStore.Migrations
{
    [DbContext(typeof(SkillitoryDbContext))]
    [Migration("20240817025440_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claim");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claim_role_id");

                    b.ToTable("role_claim", "auth");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claim");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claim_user_id");

                    b.ToTable("user_claim", "auth");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_login");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_login_user_id");

                    b.ToTable("user_login", "auth");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_role");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_user_role_role_id");

                    b.ToTable("user_role", "auth");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_token");

                    b.ToTable("user_token", "auth");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Audit.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AuditLogTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("audit_log_type_id");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_stamp");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_audit_log");

                    b.HasIndex("AuditLogTypeId")
                        .HasDatabaseName("ix_audit_log_audit_log_type_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_audit_log_user_id");

                    b.ToTable("audit_log", "audit");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Audit.AuditLogMetaData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AuditLogId")
                        .HasColumnType("integer")
                        .HasColumnName("audit_log_id");

                    b.Property<string>("MetaData")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("meta_data");

                    b.HasKey("Id")
                        .HasName("pk_audit_log_metadata");

                    b.HasIndex("AuditLogId")
                        .HasDatabaseName("ix_audit_log_metadata_audit_log_id");

                    b.ToTable("audit_log_metadata", "audit");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Audit.AuditLogType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_audit_log_type");

                    b.ToTable("audit_log_type", "audit");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SignIn"
                        },
                        new
                        {
                            Id = 2,
                            Name = "SignOut"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ForgotPassword"
                        },
                        new
                        {
                            Id = 4,
                            Name = "ResetPassword"
                        },
                        new
                        {
                            Id = 5,
                            Name = "NewUserRegistered"
                        },
                        new
                        {
                            Id = 6,
                            Name = "NewUserEmailVerified"
                        });
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Auth.SkillitoryRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<bool>("IsApplicationAdministratorRole")
                        .HasColumnType("boolean")
                        .HasColumnName("is_application_administrator_role");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.HasKey("Id")
                        .HasName("pk_role");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("role", "auth");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "224106c8-cd00-4987-99bd-7c3a0278ac09",
                            Description = "Users in this role can read and write all Skillitory resources, including customer data.",
                            IsApplicationAdministratorRole = true,
                            Name = "Skillitory Administrator",
                            NormalizedName = "SKILLITORY ADMINISTRATOR"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "f48907eb-dbf7-4fbc-be3e-9a6104c8aedc",
                            Description = "Users in this role can read all Skillitory resources, including customer data.",
                            IsApplicationAdministratorRole = true,
                            Name = "Skillitory Viewer",
                            NormalizedName = "SKILLITORY VIEWER"
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyStamp = "4caea441-7fb6-4171-83fd-0ca5e56c1757",
                            Description = "Users in this role can administrate the organizations that they're associated with.",
                            IsApplicationAdministratorRole = false,
                            Name = "Organization Administrator",
                            NormalizedName = "ORGANIZATION ADMINISTRATOR"
                        },
                        new
                        {
                            Id = 4,
                            ConcurrencyStamp = "2d4b4552-9895-4c4e-8fe4-17cae31e6848",
                            Description = "Users in this role can view the details and users of the organizations that they're associated with.",
                            IsApplicationAdministratorRole = false,
                            Name = "Organization Viewer",
                            NormalizedName = "ORGANIZATION VIEWER"
                        },
                        new
                        {
                            Id = 5,
                            ConcurrencyStamp = "1642f188-2f38-4b50-87ac-8699ce74bb4c",
                            Description = "Users in this role are standard users that can manage their own profile, skills, goals, etc.",
                            IsApplicationAdministratorRole = false,
                            Name = "User",
                            NormalizedName = "USER"
                        });
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<int?>("AvatarStoredFileId")
                        .HasColumnType("integer")
                        .HasColumnName("avatar_stored_file_id");

                    b.Property<string>("Biography")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)")
                        .HasColumnName("biography");

                    b.Property<DateOnly?>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("birth_date");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer")
                        .HasColumnName("department_id");

                    b.Property<string>("Education")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("education");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("ExternalId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("external_id");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsSignInAllowed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_sign_in_allowed");

                    b.Property<bool>("IsSystemUser")
                        .HasColumnType("boolean")
                        .HasColumnName("is_system_user");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<int?>("OrganizationId")
                        .HasColumnType("integer")
                        .HasColumnName("organization_id");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text")
                        .HasColumnName("refresh_token");

                    b.Property<DateTimeOffset?>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("refresh_token_expiry_time");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<int?>("SupervisorId")
                        .HasColumnType("integer")
                        .HasColumnName("supervisor_id");

                    b.Property<DateTimeOffset?>("TerminatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("terminated_on");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("title");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("updated_by");

                    b.Property<DateTimeOffset?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.Property<string>("UserUniqueKey")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_unique_key");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("SupervisorId")
                        .HasDatabaseName("ix_user_supervisor_id");

                    b.HasIndex("UserUniqueKey")
                        .IsUnique()
                        .HasDatabaseName("ix_user_user_unique_key");

                    b.ToTable("user", "auth");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "2c42e6b8-c307-4036-b36a-5c415b37800d",
                            CreatedBy = 1,
                            CreatedOn = new DateTimeOffset(new DateTime(2024, 8, 17, 2, 54, 40, 521, DateTimeKind.Unspecified).AddTicks(7250), new TimeSpan(0, 0, 0, 0, 0)),
                            Email = "system_user@skillitory.com",
                            EmailConfirmed = false,
                            FirstName = "SYSTEM",
                            IsSignInAllowed = false,
                            IsSystemUser = true,
                            LastName = "USER",
                            LockoutEnabled = false,
                            NormalizedEmail = "SYSTEM_USER@SKILLITORY.COM",
                            NormalizedUserName = "SYSTEM_USER@SKILLITORY.COM",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "NEVER_GOING_TO_SIGN_IN",
                            TwoFactorEnabled = false,
                            UserName = "system_user@skillitory.com",
                            UserUniqueKey = "e3vojre7i8ro3bjsh10eu64t"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_role_claim_asp_net_roles_role_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claim_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_login_user_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_role_role_id");

                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_user_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_token_user_user_id");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Audit.AuditLog", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Audit.AuditLogType", "AuditLogType")
                        .WithMany()
                        .HasForeignKey("AuditLogTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_audit_log_audit_log_types_audit_log_type_id");

                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_audit_log_asp_net_users_user_id");

                    b.Navigation("AuditLogType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Audit.AuditLogMetaData", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Audit.AuditLog", "AuditLog")
                        .WithMany()
                        .HasForeignKey("AuditLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_audit_log_metadata_audit_log_audit_log_id");

                    b.Navigation("AuditLog");
                });

            modelBuilder.Entity("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", b =>
                {
                    b.HasOne("Skillitory.Api.DataStore.Entities.Auth.SkillitoryUser", "Supervisor")
                        .WithMany()
                        .HasForeignKey("SupervisorId")
                        .HasConstraintName("fk_user_user_supervisor_id");

                    b.Navigation("Supervisor");
                });
#pragma warning restore 612, 618
        }
    }
}
