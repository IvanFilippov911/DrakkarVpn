using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerSpeedMbps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "speed_mbps",
                table: "peers",
                type: "double precision",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "speed_mbps",
                table: "peer_metrics_history",
                type: "double precision",
                precision: 10,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "speed_mbps",
                table: "peers");

            migrationBuilder.DropColumn(
                name: "speed_mbps",
                table: "peer_metrics_history");
        }
    }
}
