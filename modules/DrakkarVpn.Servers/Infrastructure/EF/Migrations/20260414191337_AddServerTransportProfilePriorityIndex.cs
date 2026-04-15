using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddServerTransportProfilePriorityIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_server_transport_profiles_server_id_priority",
                schema: "servers",
                table: "server_transport_profiles",
                columns: new[] { "server_id", "priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_server_transport_profiles_server_id_priority",
                schema: "servers",
                table: "server_transport_profiles");
        }
    }
}
