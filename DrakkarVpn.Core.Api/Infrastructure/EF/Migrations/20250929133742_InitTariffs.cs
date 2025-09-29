using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitTariffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tariffs",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Tariffs");

            migrationBuilder.RenameTable(
                name: "Tariffs",
                newName: "tariffs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tariffs",
                table: "tariffs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tariffs",
                table: "tariffs");

            migrationBuilder.RenameTable(
                name: "tariffs",
                newName: "Tariffs");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Tariffs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tariffs",
                table: "Tariffs",
                column: "Id");
        }
    }
}
