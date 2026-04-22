using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetworkMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeSchemaForProbeNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "probe_nodes",
                schema: "servers",
                newName: "probe_nodes",
                newSchema: "network_monitoring");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "servers");

            migrationBuilder.RenameTable(
                name: "probe_nodes",
                schema: "network_monitoring",
                newName: "probe_nodes",
                newSchema: "servers");
        }
    }
}
