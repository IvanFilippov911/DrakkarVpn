using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResultJson = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyKeys", x => x.Id);
                });

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
                name: "tariffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    max_devices = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscriptions_app_users_user_id",
                        column: x => x.user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    device_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.device_id);
                    table.ForeignKey(
                        name: "FK_devices_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "peers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_peer_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    config_raw = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    device_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    last_handshake_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peers", x => x.id);
                    table.ForeignKey(
                        name: "FK_peers_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "device_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_peers_servers_server_id",
                        column: x => x.server_id,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_createdat_desc_id_desc",
                table: "app_users",
                columns: new[] { "created_at", "id" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_users_status_createdat_desc_id_desc",
                table: "app_users",
                columns: new[] { "status", "created_at", "id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "ux_users_telegram_id",
                table: "app_users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_sub_createdat_desc",
                table: "devices",
                columns: new[] { "subscription_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_devices_subscription",
                table: "devices",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyKeys_ActorKey_Action_RequestId",
                table: "IdempotencyKeys",
                columns: new[] { "ActorKey", "Action", "RequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_id",
                table: "peers",
                column: "server_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_servers_health_reachable",
                table: "servers",
                column: "health_reachable");

            migrationBuilder.CreateIndex(
                name: "ix_servers_region",
                table: "servers",
                column: "region");

            migrationBuilder.CreateIndex(
                name: "ix_servers_status",
                table: "servers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_status_endat",
                table: "subscriptions",
                columns: new[] { "user_id", "status", "end_at" });

            migrationBuilder.CreateIndex(
                name: "ux_subscriptions_user_active",
                table: "subscriptions",
                column: "user_id",
                unique: true,
                filter: "\"status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyKeys");

            migrationBuilder.DropTable(
                name: "peers");

            migrationBuilder.DropTable(
                name: "server_metrics_history");

            migrationBuilder.DropTable(
                name: "tariffs");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "servers");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "app_users");
        }
    }
}
