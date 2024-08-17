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
                name: "auth");

            migrationBuilder.CreateTable(
                name: "otp_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_type", x => x.id);
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
                name: "user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_unique_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    is_sign_in_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_system_user = table.Column<bool>(type: "boolean", nullable: false),
                    terminated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    otp_type_id = table.Column<int>(type: "integer", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                        principalTable: "otp_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_user_supervisor_id",
                        column: x => x.supervisor_id,
                        principalSchema: "auth",
                        principalTable: "user",
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
                        principalSchema: "auth",
                        principalTable: "otp_type",
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

            migrationBuilder.InsertData(
                table: "otp_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Email" },
                    { 2, "TimeBased" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "otp_type",
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
                schema: "auth",
                table: "role",
                columns: new[] { "id", "concurrency_stamp", "description", "is_application_administrator_role", "name", "normalized_name" },
                values: new object[,]
                {
                    { 1, "ec4688ef-8c8c-4f52-9242-2c18d15c691e", "Users in this role can read and write all Skillitory resources, including customer data.", true, "Skillitory Administrator", "SKILLITORY ADMINISTRATOR" },
                    { 2, "30f7c420-a68a-4bac-998a-d782b3f6a76b", "Users in this role can read all Skillitory resources, including customer data.", true, "Skillitory Viewer", "SKILLITORY VIEWER" },
                    { 3, "f593af11-9315-4e66-aa3f-aaf962929c8d", "Users in this role can administrate the organizations that they're associated with.", false, "Organization Administrator", "ORGANIZATION ADMINISTRATOR" },
                    { 4, "d6c833e8-bc9a-41a7-86fe-6162b7b5f0c2", "Users in this role can view the details and users of the organizations that they're associated with.", false, "Organization Viewer", "ORGANIZATION VIEWER" },
                    { 5, "5224c3f4-7f18-4a04-b368-f533dd852782", "Users in this role are standard users that can manage their own profile, skills, goals, etc.", false, "User", "USER" }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "user",
                columns: new[] { "id", "access_failed_count", "avatar_stored_file_id", "biography", "birth_date", "concurrency_stamp", "created_by", "created_on", "department_id", "education", "email", "email_confirmed", "external_id", "first_name", "is_sign_in_allowed", "is_system_user", "last_name", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "organization_id", "otp_type_id", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "supervisor_id", "terminated_on", "title", "two_factor_enabled", "updated_by", "updated_on", "user_name", "user_unique_key" },
                values: new object[] { 1, 0, null, null, null, "5ee6e591-17cf-4937-b76d-d6f73416e4ad", 1, new DateTimeOffset(new DateTime(2024, 8, 17, 20, 41, 22, 50, DateTimeKind.Unspecified).AddTicks(7910), new TimeSpan(0, 0, 0, 0, 0)), null, null, "system_user@skillitory.com", false, null, "SYSTEM", false, true, "USER", false, null, "SYSTEM_USER@SKILLITORY.COM", "SYSTEM_USER@SKILLITORY.COM", null, null, null, null, false, null, null, "NEVER_GOING_TO_SIGN_IN", null, null, null, false, null, null, "system_user@skillitory.com", "yb7fqpc4a05t3j226lfuwuw6" });

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
                name: "ix_user_supervisor_id",
                schema: "auth",
                table: "user",
                column: "supervisor_id");

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
                name: "role_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_login",
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
                name: "role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "otp_type",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "otp_type");
        }
    }
}
