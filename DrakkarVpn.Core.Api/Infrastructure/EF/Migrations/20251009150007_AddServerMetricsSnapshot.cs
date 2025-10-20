using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddServerMetricsSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "metrics_infra_latency_ms",
                table: "servers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "metrics_packet_loss_percent",
                table: "servers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "metrics_rx_bytes",
                table: "servers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "metrics_tx_bytes",
                table: "servers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "metrics_updated_at",
                table: "servers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "metrics_vpn_latency_ms",
                table: "servers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "metrics_vpn_speed_mbps",
                table: "servers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metrics_infra_latency_ms",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_packet_loss_percent",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_rx_bytes",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_tx_bytes",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_updated_at",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_vpn_latency_ms",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_vpn_speed_mbps",
                table: "servers");
        }
    }
}
