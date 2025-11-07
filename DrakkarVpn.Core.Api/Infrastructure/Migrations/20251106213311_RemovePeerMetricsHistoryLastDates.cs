using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovePeerMetricsHistoryLastDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_data_at",
                table: "peer_metrics_history");

            migrationBuilder.DropColumn(
                name: "last_latency_at",
                table: "peer_metrics_history");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_data_at",
                table: "peer_metrics_history",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_latency_at",
                table: "peer_metrics_history",
                type: "timestamptz",
                nullable: true);
        }
    }
}
