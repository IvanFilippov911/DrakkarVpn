using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Migrations
{
    /// <inheritdoc />
    public partial class AddScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "servers");

            migrationBuilder.RenameTable(
                name: "servers",
                newName: "servers",
                newSchema: "servers");

            migrationBuilder.RenameTable(
                name: "server_realtime_stats",
                newName: "server_realtime_stats",
                newSchema: "servers");

            migrationBuilder.RenameTable(
                name: "server_poll_states",
                newName: "server_poll_states",
                newSchema: "servers");

            migrationBuilder.RenameTable(
                name: "server_metrics_history",
                newName: "server_metrics_history",
                newSchema: "servers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "servers",
                schema: "servers",
                newName: "servers");

            migrationBuilder.RenameTable(
                name: "server_realtime_stats",
                schema: "servers",
                newName: "server_realtime_stats");

            migrationBuilder.RenameTable(
                name: "server_poll_states",
                schema: "servers",
                newName: "server_poll_states");

            migrationBuilder.RenameTable(
                name: "server_metrics_history",
                schema: "servers",
                newName: "server_metrics_history");
        }
    }
}
