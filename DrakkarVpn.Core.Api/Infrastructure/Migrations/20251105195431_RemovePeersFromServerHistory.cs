using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovePeersFromServerHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_peers",
                table: "server_metrics_history");

            migrationBuilder.DropColumn(
                name: "peers_active",
                table: "server_metrics_history");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "max_peers",
                table: "server_metrics_history",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "peers_active",
                table: "server_metrics_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
