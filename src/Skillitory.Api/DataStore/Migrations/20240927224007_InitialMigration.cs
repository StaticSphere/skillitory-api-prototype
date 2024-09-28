using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Skillitory.Api.DataStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.EnsureSchema(
                name: "com");

            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "mbr");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "audit_log_type",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication_template_type",
                schema: "com",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communication_template_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization_churn_category",
                schema: "org",
                columns: table => new
                {
                    organization_churn_category_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_churn_category", x => x.organization_churn_category_id);
                });

            migrationBuilder.CreateTable(
                name: "otp_type",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_application_administrator_role = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication_template",
                schema: "com",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    communication_template_type_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    template = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communication_template", x => x.id);
                    table.ForeignKey(
                        name: "fk_communication_template_communication_template_types_communi",
                        column: x => x.communication_template_type_id,
                        principalSchema: "com",
                        principalTable: "communication_template_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_unique_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_sign_in_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_system_user = table.Column<bool>(type: "boolean", nullable: false),
                    last_sign_in_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    terminated_on_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    otp_type_id = table.Column<int>(type: "integer", nullable: true),
                    last_password_changed_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    password_expiration_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_otp_type_otp_type_id",
                        column: x => x.otp_type_id,
                        principalSchema: "auth",
                        principalTable: "otp_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claim_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    audit_log_type_id = table.Column<int>(type: "integer", nullable: false),
                    time_stamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_log_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_audit_log_audit_log_types_audit_log_type_id",
                        column: x => x.audit_log_type_id,
                        principalSchema: "audit",
                        principalTable: "audit_log_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                schema: "org",
                columns: table => new
                {
                    organization_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_unique_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    external_id_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    logo_stored_file_id = table.Column<int>(type: "integer", nullable: true),
                    is_logo_override_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_system_organization = table.Column<bool>(type: "boolean", nullable: false),
                    previous_tracked_password_count = table.Column<int>(type: "integer", nullable: true),
                    password_lifetime_days = table.Column<int>(type: "integer", nullable: true),
                    trial_period_ends_on = table.Column<DateOnly>(type: "date", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.organization_id);
                    table.ForeignKey(
                        name: "fk_organization_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_user_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "password_history",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_password_history", x => new { x.user_id, x.password_hash });
                    table.ForeignKey(
                        name: "fk_password_history_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claim_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "auth",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_login_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_token",
                schema: "auth",
                columns: table => new
                {
                    unique_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    jti = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expiration_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_refresh_token", x => x.unique_key);
                    table.ForeignKey(
                        name: "fk_user_refresh_token_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_role_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_role_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_token_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_log_metadata",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    audit_log_id = table.Column<int>(type: "integer", nullable: false),
                    meta_data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log_metadata", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_log_metadata_audit_log_audit_log_id",
                        column: x => x.audit_log_id,
                        principalSchema: "audit",
                        principalTable: "audit_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "department",
                schema: "org",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    department_unique_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    logo_stored_file_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department", x => x.department_id);
                    table.ForeignKey(
                        name: "fk_department_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "org",
                        principalTable: "organization",
                        principalColumn: "organization_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_department_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_department_user_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "organization_churn",
                schema: "org",
                columns: table => new
                {
                    organization_churn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    organization_churn_category_id = table.Column<int>(type: "integer", nullable: false),
                    details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_churned = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_churn", x => x.organization_churn_id);
                    table.ForeignKey(
                        name: "fk_organization_churn_organization_churn_category_organization",
                        column: x => x.organization_churn_category_id,
                        principalSchema: "org",
                        principalTable: "organization_churn_category",
                        principalColumn: "organization_churn_category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_churn_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "org",
                        principalTable: "organization",
                        principalColumn: "organization_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member",
                schema: "mbr",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    department_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    supervisor_id = table.Column<int>(type: "integer", nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    biography = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    education = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    external_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    avatar_stored_file_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_member", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_member_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "org",
                        principalTable: "department",
                        principalColumn: "department_id");
                    table.ForeignKey(
                        name: "fk_member_member_supervisor_id",
                        column: x => x.supervisor_id,
                        principalSchema: "mbr",
                        principalTable: "member",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_member_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "org",
                        principalTable: "organization",
                        principalColumn: "organization_id");
                    table.ForeignKey(
                        name: "fk_member_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "audit",
                table: "audit_log_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "SignIn" },
                    { 2, "SignOut" },
                    { 3, "ForgotPassword" },
                    { 4, "ResetPassword" },
                    { 5, "NewUserRegistered" },
                    { 6, "NewUserEmailVerified" }
                });

            migrationBuilder.InsertData(
                schema: "com",
                table: "communication_template_type",
                columns: new[] { "id", "name" },
                values: new object[] { 1, "Email" });

            migrationBuilder.InsertData(
                schema: "org",
                table: "organization_churn_category",
                columns: new[] { "organization_churn_category_id", "description", "name" },
                values: new object[,]
                {
                    { 1, "The user has churned because they were unhappy with some element of the application.", "Unhappy with Application" },
                    { 2, "The user has churned because they were unhappy with service.", "Unhappy with Service" },
                    { 3, "The user has churned because they no longer feel that the product is cost effective.", "No Longer Price Effective" },
                    { 4, "The user has churned for a reason other than that provided by the other churn options.", "Other" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "otp_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Email" },
                    { 2, "TimeBased" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "role",
                columns: new[] { "id", "concurrency_stamp", "description", "is_application_administrator_role", "name", "normalized_name" },
                values: new object[,]
                {
                    { 1, "660f7645-1641-4d30-afe2-55f8269b82a0", "Users in this role can read and write all Skillitory resources, including customer data.", true, "Skillitory Administrator", "SKILLITORY ADMINISTRATOR" },
                    { 2, "a86b00b9-35c3-44cf-8fab-18f5a25db6b8", "Users in this role can read all Skillitory resources, including customer data.", true, "Skillitory Viewer", "SKILLITORY VIEWER" },
                    { 3, "999ecc41-e326-4b10-bb1e-143995359ffb", "Users in this role can administrate the organizations that they're associated with.", false, "Organization Administrator", "ORGANIZATION ADMINISTRATOR" },
                    { 4, "1f4b121c-5200-4dc1-a302-38ef3aeefcbd", "Users in this role can view the details and users of the organizations that they're associated with.", false, "Organization Viewer", "ORGANIZATION VIEWER" },
                    { 5, "1ce3e6ea-779d-4e5e-bb8c-b5500bd3eab7", "Users in this role are standard users that can manage their own profile, skills, goals, etc.", false, "User", "USER" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "user",
                columns: new[] { "id", "access_failed_count", "concurrency_stamp", "created_by", "created_date_time", "email", "email_confirmed", "is_sign_in_allowed", "is_system_user", "last_password_changed_date_time", "last_sign_in_date_time", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "otp_type_id", "password_expiration_date_time", "password_hash", "phone_number", "phone_number_confirmed", "security_stamp", "terminated_on_date_time", "two_factor_enabled", "updated_by", "updated_date_time", "user_name", "user_unique_key" },
                values: new object[] { 1, 0, "959aacd0-b43f-48d6-82b7-1f52d8b03a2b", 1, new DateTimeOffset(new DateTime(2024, 9, 27, 22, 40, 7, 505, DateTimeKind.Unspecified).AddTicks(6980), new TimeSpan(0, 0, 0, 0, 0)), "system_user@skillitory.com", false, false, true, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, "SYSTEM_USER@SKILLITORY.COM", "SYSTEM_USER@SKILLITORY.COM", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, false, "NEVER_GOING_TO_SIGN_IN", null, false, null, null, "system_user@skillitory.com", "hhhrovf9938vnu35ge0azd7u" });

            migrationBuilder.InsertData(
                schema: "com",
                table: "communication_template",
                columns: new[] { "id", "communication_template_type_id", "created_by", "created_date_time", "name", "template", "updated_by", "updated_date_time" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTimeOffset(new DateTime(2024, 9, 27, 22, 40, 7, 505, DateTimeKind.Unspecified).AddTicks(7200), new TimeSpan(0, 0, 0, 0, 0)), "ValidateSkillitoryAccount", "<!DOCTYPE html>\n<html lang=\"en\">\n  <head>\n    <meta charset=\"UTF-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\n    <title>Skillitory Communication</title>\n    <style>\n      * {\n        margin: 0;\n        padding: 0;\n        font-family: Verdana, Geneva, Tahoma, sans-serif;\n      }\n\n      .container {\n        display: flex;\n        justify-content: center;\n        align-items: center;\n        height: 100dvh;\n        background-color: gray;\n      }\n\n      .card {\n        max-width: 30rem;\n        padding: 2rem;\n        border: solid 1px darkgray;\n        border-radius: 1rem;\n        background-color: lightgray;\n        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);\n      }\n\n      .card__content > p {\n        margin: 1rem auto;\n      }\n    </style>\n  </head>\n  <body class=\"container\">\n    <main class=\"card\" role=\"main\">\n      <section class=\"card__content\">\n        <p>\n          Your user account has been created in Skillitory. Please click the\n          following link to validate your email address:\n        </p>\n        <p>\n          <a href=\"{{ callbackUrl }}\">Verify Email</a>\n        </p>\n      </section>\n    </main>\n  </body>\n</html>", null, null },
                    { 2, 1, 1, new DateTimeOffset(new DateTime(2024, 9, 27, 22, 40, 7, 505, DateTimeKind.Unspecified).AddTicks(7200), new TimeSpan(0, 0, 0, 0, 0)), "ForgotPassword", "<!DOCTYPE html>\n<html lang=\"en\">\n  <head>\n    <meta charset=\"UTF-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\n    <title>Skillitory Communication</title>\n    <style>\n      * {\n        margin: 0;\n        padding: 0;\n        font-family: Verdana, Geneva, Tahoma, sans-serif;\n      }\n\n      .container {\n        display: flex;\n        justify-content: center;\n        align-items: center;\n        height: 100dvh;\n        background-color: gray;\n      }\n\n      .card {\n        max-width: 30rem;\n        padding: 2rem;\n        border: solid 1px darkgray;\n        border-radius: 1rem;\n        background-color: lightgray;\n        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);\n      }\n\n      .card__content > p {\n        margin: 1rem auto;\n      }\n    </style>\n  </head>\n  <body class=\"container\">\n    <main class=\"card\" role=\"main\">\n      <section class=\"card__content\">\n        <p>\n          A request has been sent to set or reset your Skillitory password.\n          Please click the link below, which will take you to the Skillitory\n          reset password screen.\n        </p>\n        <p>\n          Please note that your new password must abide by the following rules:\n        </p>\n        <ul>\n          <li>Must be at least 8 characters</li>\n          <li>At least 1 uppercase letter</li>\n          <li>At least 1 lowercase letter</li>\n          <li>At least 1 number</li>\n          <li>At least 1 symbol</li>\n        </ul>\n        <p>\n          <strong\n            ><a href=\"{{ callbackUrl }}\" target=\"_blank\">\n              Please click here to go to the reset password screen.</a\n            >\n          </strong>\n          <br />\n        </p>\n      </section>\n    </main>\n  </body>\n</html>", null, null },
                    { 3, 1, 1, new DateTimeOffset(new DateTime(2024, 9, 27, 22, 40, 7, 505, DateTimeKind.Unspecified).AddTicks(7210), new TimeSpan(0, 0, 0, 0, 0)), "SignInOtp", "<!DOCTYPE html>\n<html lang=\"en\">\n  <head>\n    <meta charset=\"UTF-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\n    <title>Skillitory Communication</title>\n    <style>\n      * {\n        margin: 0;\n        padding: 0;\n        font-family: Verdana, Geneva, Tahoma, sans-serif;\n      }\n\n      .container {\n        display: flex;\n        justify-content: center;\n        align-items: center;\n        height: 100dvh;\n        background-color: gray;\n      }\n\n      .card {\n        max-width: 30rem;\n        padding: 2rem;\n        border: solid 1px darkgray;\n        border-radius: 1rem;\n        background-color: lightgray;\n        box-shadow: 0 0 20px 1px rgba(0, 0, 0, 0.5);\n      }\n\n      .card__content > p {\n        margin: 1rem auto;\n      }\n    </style>\n  </head>\n  <body class=\"container\">\n    <main class=\"card\" role=\"main\">\n      <section class=\"card__content\">\n        <p>\n          Please enter the following one time password in Skillitory to complete\n          your sign in!\n        </p>\n        <h1>{{ otp }}</h1>\n      </section>\n    </main>\n  </body>\n</html>", null, null }
                });

            migrationBuilder.InsertData(
                schema: "org",
                table: "organization",
                columns: new[] { "organization_id", "created_by", "created_date_time", "description", "external_id_name", "is_logo_override_allowed", "is_system_organization", "logo_stored_file_id", "name", "notes", "organization_unique_key", "password_lifetime_days", "previous_tracked_password_count", "trial_period_ends_on", "updated_by", "updated_date_time" },
                values: new object[] { 1, 1, new DateTimeOffset(new DateTime(2024, 9, 27, 22, 40, 7, 505, DateTimeKind.Unspecified).AddTicks(7160), new TimeSpan(0, 0, 0, 0, 0)), "The organization that owns and developed Skillitory.", null, false, true, null, "StaticSphere", null, "ql6a0bxhhwh1o3dyzt7cer17", null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_audit_log_type_id",
                schema: "audit",
                table: "audit_log",
                column: "audit_log_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_user_id",
                schema: "audit",
                table: "audit_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_metadata_audit_log_id",
                schema: "audit",
                table: "audit_log_metadata",
                column: "audit_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_communication_template_communication_template_type_id",
                schema: "com",
                table: "communication_template",
                column: "communication_template_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_department_created_by",
                schema: "org",
                table: "department",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_department_department_unique_key",
                schema: "org",
                table: "department",
                column: "department_unique_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_department_organization_id_name",
                schema: "org",
                table: "department",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_department_updated_by",
                schema: "org",
                table: "department",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_member_department_id",
                schema: "mbr",
                table: "member",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_organization_id",
                schema: "mbr",
                table: "member",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_supervisor_id",
                schema: "mbr",
                table: "member",
                column: "supervisor_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_created_by",
                schema: "org",
                table: "organization",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_organization_name",
                schema: "org",
                table: "organization",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_organization_unique_key",
                schema: "org",
                table: "organization",
                column: "organization_unique_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_updated_by",
                schema: "org",
                table: "organization",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_organization_churn_organization_churn_category_id",
                schema: "org",
                table: "organization_churn",
                column: "organization_churn_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_churn_organization_id",
                schema: "org",
                table: "organization_churn",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_password_history_user_id",
                schema: "auth",
                table: "password_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_claim_role_id",
                schema: "auth",
                table: "role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "auth",
                table: "user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_user_otp_type_id",
                schema: "auth",
                table: "user",
                column: "otp_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_user_unique_key",
                schema: "auth",
                table: "user",
                column: "user_unique_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "auth",
                table: "user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claim_user_id",
                schema: "auth",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_login_user_id",
                schema: "auth",
                table: "user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_token_user_id_jti",
                schema: "auth",
                table: "user_refresh_token",
                columns: new[] { "user_id", "jti" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_token_user_id_token",
                schema: "auth",
                table: "user_refresh_token",
                columns: new[] { "user_id", "token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_role_id",
                schema: "auth",
                table: "user_role",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log_metadata",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "communication_template",
                schema: "com");

            migrationBuilder.DropTable(
                name: "member",
                schema: "mbr");

            migrationBuilder.DropTable(
                name: "organization_churn",
                schema: "org");

            migrationBuilder.DropTable(
                name: "password_history",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_login",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_refresh_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "communication_template_type",
                schema: "com");

            migrationBuilder.DropTable(
                name: "department",
                schema: "org");

            migrationBuilder.DropTable(
                name: "organization_churn_category",
                schema: "org");

            migrationBuilder.DropTable(
                name: "role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "audit_log_type",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "organization",
                schema: "org");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "otp_type",
                schema: "auth");
        }
    }
}
