using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ban_reason",
                table: "app_users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "banned_at_utc",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_internal",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_peer_hist_srv_period_online",
                table: "peer_metrics_history",
                columns: new[] { "server_id", "period_start", "is_online" });

            migrationBuilder.CreateIndex(
                name: "ix_users_is_internal",
                table: "app_users",
                column: "is_internal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_peer_hist_srv_period_online",
                table: "peer_metrics_history");

            migrationBuilder.DropIndex(
                name: "ix_users_is_internal",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "ban_reason",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "banned_at_utc",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "is_internal",
                table: "app_users");
        }
    }
}
