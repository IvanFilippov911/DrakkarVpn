using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idempotency.Migrations
{
    /// <inheritdoc />
    public partial class AddScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "idempotency");

            migrationBuilder.RenameTable(
                name: "IdempotencyKeys",
                newName: "IdempotencyKeys",
                newSchema: "idempotency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "IdempotencyKeys",
                schema: "idempotency",
                newName: "IdempotencyKeys");
        }
    }
}
