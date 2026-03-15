using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DrakkarVpn.AdminAuth.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admin_auth");

            migrationBuilder.CreateTable(
                name: "admin_roles",
                schema: "admin_auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admin_users",
                schema: "admin_auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_admin_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admin_role_claims",
                schema: "admin_auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_role_claims_admin_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_refresh_tokens",
                schema: "admin_auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_by_ip = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_refresh_tokens_admin_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_user_claims",
                schema: "admin_auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_user_claims_admin_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_user_logins",
                schema: "admin_auth",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_admin_user_logins_admin_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_user_roles",
                schema: "admin_auth",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_admin_user_roles_admin_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_admin_user_roles_admin_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_user_tokens",
                schema: "admin_auth",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_admin_user_tokens_admin_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "admin_auth",
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_refresh_tokens_admin_user_active_window",
                schema: "admin_auth",
                table: "admin_refresh_tokens",
                columns: new[] { "admin_user_id", "revoked_at_utc", "expires_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_admin_refresh_tokens_expires_at_utc",
                schema: "admin_auth",
                table: "admin_refresh_tokens",
                column: "expires_at_utc");

            migrationBuilder.CreateIndex(
                name: "ux_admin_refresh_tokens_token_hash",
                schema: "admin_auth",
                table: "admin_refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_role_claims_role_id",
                schema: "admin_auth",
                table: "admin_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "admin_auth",
                table: "admin_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_user_claims_user_id",
                schema: "admin_auth",
                table: "admin_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_user_logins_user_id",
                schema: "admin_auth",
                table: "admin_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_user_roles_role_id",
                schema: "admin_auth",
                table: "admin_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "admin_auth",
                table: "admin_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_email",
                schema: "admin_auth",
                table: "admin_users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_is_active",
                schema: "admin_auth",
                table: "admin_users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "admin_auth",
                table: "admin_users",
                column: "normalized_user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_refresh_tokens",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_role_claims",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_user_claims",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_user_logins",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_user_roles",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_user_tokens",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_roles",
                schema: "admin_auth");

            migrationBuilder.DropTable(
                name: "admin_users",
                schema: "admin_auth");
        }
    }
}
