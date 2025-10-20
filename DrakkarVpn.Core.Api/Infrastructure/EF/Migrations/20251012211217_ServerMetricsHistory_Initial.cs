using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class ServerMetricsHistory_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "server_metrics_history",
                columns: table => new
                {
                    period_start = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reachable = table.Column<bool>(type: "boolean", nullable: false),
                    peers_active = table.Column<int>(type: "integer", nullable: false),
                    max_peers = table.Column<int>(type: "integer", nullable: true),
                    traffic_rx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    traffic_tx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    vpn_speed_mbps = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    infra_latency_ms = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_metrics_history", x => new { x.period_start, x.server_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_metrics_history");
        }
    }
}
