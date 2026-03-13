using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Users.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.CreateTable(
                name: "app_users",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false),
                    ban_reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    banned_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    moderation_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_realtime_stats",
                schema: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_end_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_subscription_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_subscription_status = table.Column<int>(type: "integer", nullable: true),
                    subscription_max_devices = table.Column<int>(type: "integer", nullable: false),
                    device_count = table.Column<int>(type: "integer", nullable: false),
                    last_seen_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    traffic_24h_bytes = table.Column<long>(type: "bigint", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_realtime_stats", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "users",
                columns: table => new
                {
                    device_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    status_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.device_id);
                    table.ForeignKey(
                        name: "FK_devices_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_createdat_desc_id_desc",
                schema: "users",
                table: "app_users",
                columns: new[] { "created_at", "id" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_users_is_internal",
                schema: "users",
                table: "app_users",
                column: "is_internal");

            migrationBuilder.CreateIndex(
                name: "ix_users_moderation_marker_status",
                schema: "users",
                table: "app_users",
                columns: new[] { "moderation_updated_at_utc", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_users_status_createdat_desc_id_desc",
                schema: "users",
                table: "app_users",
                columns: new[] { "status", "created_at", "id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "ux_users_telegram_id",
                schema: "users",
                table: "app_users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_sub_createdat_desc",
                schema: "users",
                table: "devices",
                columns: new[] { "user_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_devices_sub_deviceid",
                schema: "users",
                table: "devices",
                columns: new[] { "user_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_subscription",
                schema: "users",
                table: "devices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_user_statusupdatedat",
                schema: "users",
                table: "devices",
                columns: new[] { "user_id", "status_updated_at_utc" });

            migrationBuilder.CreateIndex(
                name: "idx_userstats_is_online",
                schema: "users",
                table: "user_realtime_stats",
                column: "is_online");

            migrationBuilder.CreateIndex(
                name: "idx_userstats_lastseen",
                schema: "users",
                table: "user_realtime_stats",
                column: "last_seen_utc");

            migrationBuilder.CreateIndex(
                name: "idx_userstats_sub_active",
                schema: "users",
                table: "user_realtime_stats",
                column: "is_subscription_active");

            migrationBuilder.CreateIndex(
                name: "idx_userstats_sub_end",
                schema: "users",
                table: "user_realtime_stats",
                column: "subscription_end_utc");

            migrationBuilder.CreateIndex(
                name: "idx_userstats_traffic24h",
                schema: "users",
                table: "user_realtime_stats",
                column: "traffic_24h_bytes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices",
                schema: "users");

            migrationBuilder.DropTable(
                name: "user_realtime_stats",
                schema: "users");

            migrationBuilder.DropTable(
                name: "app_users",
                schema: "users");
        }
    }
}
