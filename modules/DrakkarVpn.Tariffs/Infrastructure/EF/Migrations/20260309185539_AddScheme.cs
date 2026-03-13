using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Tariffs.Migrations
{
    /// <inheritdoc />
    public partial class AddScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tariffs");

            migrationBuilder.RenameTable(
                name: "tariffs",
                newName: "tariffs",
                newSchema: "tariffs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "tariffs",
                schema: "tariffs",
                newName: "tariffs");
        }
    }
}
