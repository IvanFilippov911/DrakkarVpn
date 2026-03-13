using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialServers : Migration
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
                    traffic_rx_delta_bytes = table.Column<long>(type: "bigint", nullable: false),
                    traffic_tx_delta_bytes = table.Column<long>(type: "bigint", nullable: false),
                    vpn_speed_mbps = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    infra_latency_ms = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_metrics_history", x => new { x.period_start, x.server_id });
                });

            migrationBuilder.CreateTable(
                name: "server_realtime_stats",
                columns: table => new
                {
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    online_peers = table.Column<int>(type: "integer", nullable: false),
                    traffic_last_1h_bytes = table.Column<long>(type: "bigint", nullable: false),
                    traffic_last_24h_bytes = table.Column<long>(type: "bigint", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    traffic_calculated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_realtime_stats", x => x.server_id);
                });

            migrationBuilder.CreateTable(
                name: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    region = table.Column<string>(type: "text", nullable: false),
                    public_host = table.Column<string>(type: "text", nullable: false),
                    agent_base_url = table.Column<string>(type: "text", nullable: false),
                    agent_token_encrypted = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    max_peers = table.Column<int>(type: "integer", nullable: true),
                    health_reachable = table.Column<bool>(type: "boolean", nullable: false),
                    health_peers_active = table.Column<int>(type: "integer", nullable: false),
                    health_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    metrics_rx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    metrics_tx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    metrics_vpn_speed_mbps = table.Column<double>(type: "double precision", nullable: false),
                    metrics_infra_latency_ms = table.Column<double>(type: "double precision", nullable: false),
                    metrics_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    benchmark_max_speed_mbps = table.Column<double>(type: "double precision", nullable: false),
                    benchmark_measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server_poll_states",
                columns: table => new
                {
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaseOwner = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LeaseUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLeaseRenewedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConsecutiveFailures = table.Column<int>(type: "integer", nullable: false),
                    BackoffUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastReachableUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPollStartedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPollFinishedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPollLatencyMs = table.Column<int>(type: "integer", nullable: true),
                    LastPollSuccess = table.Column<bool>(type: "boolean", nullable: true),
                    LastPollErrorCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LastKnownPeersActive = table.Column<int>(type: "integer", nullable: false),
                    LastRxTotal = table.Column<long>(type: "bigint", nullable: false),
                    LastTxTotal = table.Column<long>(type: "bigint", nullable: false),
                    LastTotalsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastRxDelta = table.Column<long>(type: "bigint", nullable: false),
                    LastTxDelta = table.Column<long>(type: "bigint", nullable: false),
                    LastDeltaAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdaterInstance = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_poll_states", x => x.ServerId);
                    table.ForeignKey(
                        name: "FK_server_poll_states_servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_server_metrics_server_ts",
                table: "server_metrics_history",
                columns: new[] { "server_id", "period_start" });

            migrationBuilder.CreateIndex(
                name: "IX_server_poll_states_BackoffUntilUtc",
                table: "server_poll_states",
                column: "BackoffUntilUtc");

            migrationBuilder.CreateIndex(
                name: "IX_server_poll_states_LastReachableUtc",
                table: "server_poll_states",
                column: "LastReachableUtc");

            migrationBuilder.CreateIndex(
                name: "IX_server_poll_states_LeaseUntilUtc",
                table: "server_poll_states",
                column: "LeaseUntilUtc");

            migrationBuilder.CreateIndex(
                name: "ix_servers_health_reachable",
                table: "servers",
                column: "health_reachable");

            migrationBuilder.CreateIndex(
                name: "ix_servers_region_status",
                table: "servers",
                columns: new[] { "region", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_metrics_history");

            migrationBuilder.DropTable(
                name: "server_poll_states");

            migrationBuilder.DropTable(
                name: "server_realtime_stats");

            migrationBuilder.DropTable(
                name: "servers");
        }
    }
}
