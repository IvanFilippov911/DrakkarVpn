using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActiveSubscriptionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId",
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions");
        }
    }
}
