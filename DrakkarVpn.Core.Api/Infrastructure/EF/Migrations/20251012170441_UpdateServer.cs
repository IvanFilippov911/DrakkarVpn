using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metrics_IsFallBack",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "metrics_packet_loss_percent",
                table: "servers");

            migrationBuilder.RenameColumn(
                name: "metrics_vpn_latency_ms",
                table: "servers",
                newName: "benchmark_max_speed_mbps");

            migrationBuilder.AddColumn<DateTime>(
                name: "benchmark_measured_at",
                table: "servers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "benchmark_measured_at",
                table: "servers");

            migrationBuilder.RenameColumn(
                name: "benchmark_max_speed_mbps",
                table: "servers",
                newName: "metrics_vpn_latency_ms");

            migrationBuilder.AddColumn<bool>(
                name: "Metrics_IsFallBack",
                table: "servers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "metrics_packet_loss_percent",
                table: "servers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
