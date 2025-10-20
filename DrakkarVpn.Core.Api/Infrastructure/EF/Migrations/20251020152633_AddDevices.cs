using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "peers",
                newName: "subscription_id");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "peers",
                newName: "last_handshake_at");

            migrationBuilder.AddColumn<int>(
                name: "MaxDevices",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "device_id",
                table: "peers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    device_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.device_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_peers_sub_status",
                table: "peers",
                columns: new[] { "subscription_id", "status" });

            // ⚠️ ДОБАВЛЯЕМ этот SQL-блок перед созданием уникального индекса:
            migrationBuilder.Sql(@"
                WITH d AS (
                    SELECT id,
                           ROW_NUMBER() OVER (
                               PARTITION BY subscription_id, device_id
                               ORDER BY created_at DESC
                           ) AS rn
                    FROM peers
                    WHERE status = 0
                )
                UPDATE peers p
                SET status = 1
                FROM d
                WHERE p.id = d.id
                  AND d.rn > 1;
            ");

            // ✅ теперь создаём индекс
            migrationBuilder.CreateIndex(
                name: "ux_peers_sub_device_active",
                table: "peers",
                columns: new[] { "subscription_id", "device_id" },
                unique: true,
                filter: "\"status\" = 0");

            migrationBuilder.CreateIndex(
                name: "ix_devices_user",
                table: "devices",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropIndex(
                name: "ix_peers_sub_status",
                table: "peers");

            migrationBuilder.DropIndex(
                name: "ux_peers_sub_device_active",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "MaxDevices",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "device_id",
                table: "peers");

            migrationBuilder.RenameColumn(
                name: "subscription_id",
                table: "peers",
                newName: "SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "last_handshake_at",
                table: "peers",
                newName: "expires_at");
        }
    }
}
