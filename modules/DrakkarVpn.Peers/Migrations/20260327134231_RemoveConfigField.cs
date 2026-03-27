using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Peers.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConfigField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "config_raw",
                schema: "peers",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "ConfigRaw",
                schema: "peers",
                table: "peer_provision_jobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "config_raw",
                schema: "peers",
                table: "peers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfigRaw",
                schema: "peers",
                table: "peer_provision_jobs",
                type: "text",
                nullable: true);
        }
    }
}
