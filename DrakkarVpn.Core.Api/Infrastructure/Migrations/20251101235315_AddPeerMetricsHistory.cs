using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerMetricsHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_data_at",
                table: "peers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_latency_at",
                table: "peers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_polled_at",
                table: "peers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "total_rx_bytes",
                table: "peers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "total_tx_bytes",
                table: "peers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "vpn_latency_ms",
                table: "peers",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "peer_metrics_history",
                columns: table => new
                {
                    peer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    total_rx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    total_tx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    vpn_latency_ms = table.Column<double>(type: "double precision", nullable: true),
                    last_handshake_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    last_data_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    last_latency_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_metrics_history", x => new { x.period_start, x.server_id, x.peer_id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_last_handshake_desc",
                table: "peers",
                columns: new[] { "server_id", "last_handshake_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_status",
                table: "peers",
                columns: new[] { "server_id", "status" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_peers_total_rx_bytes_nonneg",
                table: "peers",
                sql: "total_rx_bytes >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_peers_total_tx_bytes_nonneg",
                table: "peers",
                sql: "total_tx_bytes >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_peers_vpn_latency_ms_valid",
                table: "peers",
                sql: "vpn_latency_ms IS NULL OR vpn_latency_ms >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_peer_metrics_history_peer_id_period_start",
                table: "peer_metrics_history",
                columns: new[] { "peer_id", "period_start" });

            migrationBuilder.CreateIndex(
                name: "IX_peer_metrics_history_period_start",
                table: "peer_metrics_history",
                column: "period_start");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peer_metrics_history");

            migrationBuilder.DropIndex(
                name: "ix_peers_server_last_handshake_desc",
                table: "peers");

            migrationBuilder.DropIndex(
                name: "ix_peers_server_status",
                table: "peers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_peers_total_rx_bytes_nonneg",
                table: "peers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_peers_total_tx_bytes_nonneg",
                table: "peers");

            migrationBuilder.DropCheckConstraint(
                name: "ck_peers_vpn_latency_ms_valid",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "last_data_at",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "last_latency_at",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "last_polled_at",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "total_rx_bytes",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "total_tx_bytes",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "vpn_latency_ms",
                table: "peers");
        }
    }
}
