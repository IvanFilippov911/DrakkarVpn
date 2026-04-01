using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Peers.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_peer_provision_jobs_device_active",
                schema: "peers",
                table: "peer_provision_jobs");

            migrationBuilder.CreateIndex(
                name: "ux_peer_provision_jobs_device_active",
                schema: "peers",
                table: "peer_provision_jobs",
                column: "DeviceId",
                unique: true,
                filter: "\"State\" <> 3 AND \"State\" <> 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_peer_provision_jobs_device_active",
                schema: "peers",
                table: "peer_provision_jobs");

            migrationBuilder.CreateIndex(
                name: "ux_peer_provision_jobs_device_active",
                schema: "peers",
                table: "peer_provision_jobs",
                column: "DeviceId",
                unique: true,
                filter: "\"State\" <> 3");
        }
    }
}
