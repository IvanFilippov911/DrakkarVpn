using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOnlineFieldOnPeer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_online",
                table: "peers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_online",
                table: "peers");
        }
    }
}
