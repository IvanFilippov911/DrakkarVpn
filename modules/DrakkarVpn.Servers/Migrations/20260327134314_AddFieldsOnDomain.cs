using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsOnDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "public_port",
                schema: "servers",
                table: "servers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "reality_public_key",
                schema: "servers",
                table: "servers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reality_short_id",
                schema: "servers",
                table: "servers",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reality_sni",
                schema: "servers",
                table: "servers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "public_port",
                schema: "servers",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "reality_public_key",
                schema: "servers",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "reality_short_id",
                schema: "servers",
                table: "servers");

            migrationBuilder.DropColumn(
                name: "reality_sni",
                schema: "servers",
                table: "servers");
        }
    }
}
