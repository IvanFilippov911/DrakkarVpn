using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Peers.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialPeers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "peer_metrics_history",
                columns: table => new
                {
                    peer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    total_rx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    total_tx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    rx_delta_bytes = table.Column<long>(type: "bigint", nullable: false),
                    tx_delta_bytes = table.Column<long>(type: "bigint", nullable: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    speed_mbps = table.Column<double>(type: "double precision", precision: 10, scale: 2, nullable: true),
                    vpn_latency_ms = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_metrics_history", x => new { x.period_start, x.server_id, x.peer_id });
                });

            migrationBuilder.CreateTable(
                name: "peer_provision_jobs",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentAppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<short>(type: "smallint", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextAttemptAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaseOwner = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LeaseUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PeerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgentPeerUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfigRaw = table.Column<string>(type: "text", nullable: true),
                    LastErrorCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastErrorMessage = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_provision_jobs", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "peer_sync_issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgentPeerUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DetectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_sync_issues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "peers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_peer_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    config_raw = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_data_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_rx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    total_tx_bytes = table.Column<long>(type: "bigint", nullable: false),
                    speed_mbps = table.Column<double>(type: "double precision", precision: 10, scale: 2, nullable: true),
                    last_polled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_online = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    vpn_latency_ms = table.Column<double>(type: "double precision", nullable: true),
                    last_latency_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peers", x => x.id);
                    table.CheckConstraint("ck_peers_total_rx_bytes_nonneg", "total_rx_bytes >= 0");
                    table.CheckConstraint("ck_peers_total_tx_bytes_nonneg", "total_tx_bytes >= 0");
                    table.CheckConstraint("ck_peers_vpn_latency_ms_valid", "vpn_latency_ms IS NULL OR vpn_latency_ms >= 0");
                });

            migrationBuilder.CreateTable(
                name: "peer_traffic_agg",
                columns: table => new
                {
                    PeerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Last1hBytes = table.Column<long>(type: "bigint", nullable: false),
                    Last24hBytes = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_traffic_agg", x => x.PeerId);
                    table.ForeignKey(
                        name: "FK_peer_traffic_agg_peers_PeerId",
                        column: x => x.PeerId,
                        principalTable: "peers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_peer_hist_srv_period_online",
                table: "peer_metrics_history",
                columns: new[] { "server_id", "period_start", "is_online" });

            migrationBuilder.CreateIndex(
                name: "IX_peer_metrics_history_peer_id_period_start",
                table: "peer_metrics_history",
                columns: new[] { "peer_id", "period_start" });

            migrationBuilder.CreateIndex(
                name: "IX_peer_metrics_history_period_start",
                table: "peer_metrics_history",
                column: "period_start");

            migrationBuilder.CreateIndex(
                name: "ix_peer_metrics_server_period_peer",
                table: "peer_metrics_history",
                columns: new[] { "server_id", "period_start", "peer_id" });

            migrationBuilder.CreateIndex(
                name: "IX_peer_provision_jobs_PeerId_AgentAppliedAtUtc",
                table: "peer_provision_jobs",
                columns: new[] { "PeerId", "AgentAppliedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "ix_peer_provision_jobs_state_next",
                table: "peer_provision_jobs",
                columns: new[] { "State", "NextAttemptAtUtc" });

            migrationBuilder.CreateIndex(
                name: "ux_peer_provision_jobs_device_active",
                table: "peer_provision_jobs",
                column: "DeviceId",
                unique: true,
                filter: "\"State\" <> 3");

            migrationBuilder.CreateIndex(
                name: "IX_peer_sync_issues_ServerId_Type_DetectedAtUtc",
                table: "peer_sync_issues",
                columns: new[] { "ServerId", "Type", "DetectedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_peer_traffic_agg_Last1hBytes",
                table: "peer_traffic_agg",
                column: "Last1hBytes");

            migrationBuilder.CreateIndex(
                name: "IX_peer_traffic_agg_Last24hBytes",
                table: "peer_traffic_agg",
                column: "Last24hBytes");

            migrationBuilder.CreateIndex(
                name: "IX_peer_traffic_agg_UpdatedAtUtc",
                table: "peer_traffic_agg",
                column: "UpdatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_id",
                table: "peers",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_lastdata",
                table: "peers",
                columns: new[] { "server_id", "last_data_at" },
                filter: "\"last_data_at\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_online",
                table: "peers",
                columns: new[] { "server_id", "is_online" },
                filter: "\"is_online\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_status",
                table: "peers",
                columns: new[] { "server_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_peers_status_statusupdatedat",
                table: "peers",
                columns: new[] { "status", "status_updated_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_peers_statusupdatedat",
                table: "peers",
                column: "status_updated_at_utc");

            migrationBuilder.CreateIndex(
                name: "ux_peers_agent_peer_uuid",
                table: "peers",
                column: "agent_peer_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_peers_device_active",
                table: "peers",
                column: "device_id",
                unique: true,
                filter: "\"status\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peer_metrics_history");

            migrationBuilder.DropTable(
                name: "peer_provision_jobs");

            migrationBuilder.DropTable(
                name: "peer_sync_issues");

            migrationBuilder.DropTable(
                name: "peer_traffic_agg");

            migrationBuilder.DropTable(
                name: "peers");
        }
    }
}
