using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerSyncIssuesAndPeerMetricsPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_peers_server_last_handshake_desc",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "last_handshake_at",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "last_handshake_at",
                table: "peer_metrics_history");

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

            migrationBuilder.CreateIndex(
                name: "IX_peer_sync_issues_ServerId_Type_DetectedAtUtc",
                table: "peer_sync_issues",
                columns: new[] { "ServerId", "Type", "DetectedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peer_sync_issues");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_handshake_at",
                table: "peers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_handshake_at",
                table: "peer_metrics_history",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_last_handshake_desc",
                table: "peers",
                columns: new[] { "server_id", "last_handshake_at" },
                descending: new[] { false, true });
        }
    }
}
