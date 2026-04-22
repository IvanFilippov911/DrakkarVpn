using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetworkMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddProbeNodeAndProbeResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "servers");

            migrationBuilder.EnsureSchema(
                name: "network_monitoring");

            migrationBuilder.CreateTable(
                name: "probe_nodes",
                schema: "servers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Region = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Host = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastSeenAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_probe_nodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "probe_results",
                schema: "network_monitoring",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProbeNodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    LatencyMs = table.Column<int>(type: "integer", nullable: true),
                    ErrorCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CheckedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_probe_results", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_probe_nodes_Host",
                schema: "servers",
                table: "probe_nodes",
                column: "Host",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_probe_nodes_IsEnabled_Status",
                schema: "servers",
                table: "probe_nodes",
                columns: new[] { "IsEnabled", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_probe_nodes_Name",
                schema: "servers",
                table: "probe_nodes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_probe_nodes_Status",
                schema: "servers",
                table: "probe_nodes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_CheckedAtUtc",
                schema: "network_monitoring",
                table: "probe_results",
                column: "CheckedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_ProbeNodeId",
                schema: "network_monitoring",
                table: "probe_results",
                column: "ProbeNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_ProbeNodeId_CheckedAtUtc",
                schema: "network_monitoring",
                table: "probe_results",
                columns: new[] { "ProbeNodeId", "CheckedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_ProfileId",
                schema: "network_monitoring",
                table: "probe_results",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_ServerId",
                schema: "network_monitoring",
                table: "probe_results",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_probe_results_ServerId_ProfileId_CheckedAtUtc",
                schema: "network_monitoring",
                table: "probe_results",
                columns: new[] { "ServerId", "ProfileId", "CheckedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "probe_nodes",
                schema: "servers");

            migrationBuilder.DropTable(
                name: "probe_results",
                schema: "network_monitoring");
        }
    }
}
