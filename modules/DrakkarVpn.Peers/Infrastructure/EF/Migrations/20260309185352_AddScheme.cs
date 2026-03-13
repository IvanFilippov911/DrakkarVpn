using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Peers.Migrations
{
    /// <inheritdoc />
    public partial class AddScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "peers");

            migrationBuilder.RenameTable(
                name: "peers",
                newName: "peers",
                newSchema: "peers");

            migrationBuilder.RenameTable(
                name: "peer_traffic_agg",
                newName: "peer_traffic_agg",
                newSchema: "peers");

            migrationBuilder.RenameTable(
                name: "peer_sync_issues",
                newName: "peer_sync_issues",
                newSchema: "peers");

            migrationBuilder.RenameTable(
                name: "peer_provision_jobs",
                newName: "peer_provision_jobs",
                newSchema: "peers");

            migrationBuilder.RenameTable(
                name: "peer_metrics_history",
                newName: "peer_metrics_history",
                newSchema: "peers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "peers",
                schema: "peers",
                newName: "peers");

            migrationBuilder.RenameTable(
                name: "peer_traffic_agg",
                schema: "peers",
                newName: "peer_traffic_agg");

            migrationBuilder.RenameTable(
                name: "peer_sync_issues",
                schema: "peers",
                newName: "peer_sync_issues");

            migrationBuilder.RenameTable(
                name: "peer_provision_jobs",
                schema: "peers",
                newName: "peer_provision_jobs");

            migrationBuilder.RenameTable(
                name: "peer_metrics_history",
                schema: "peers",
                newName: "peer_metrics_history");
        }
    }
}
