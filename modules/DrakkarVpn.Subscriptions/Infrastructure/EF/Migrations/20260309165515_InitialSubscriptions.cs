using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Subscriptions.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    status_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    max_devices = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_subs_status_statusupdatedat_id",
                table: "subscriptions",
                columns: new[] { "status", "status_updated_at_utc", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_status_endat_desc",
                table: "subscriptions",
                columns: new[] { "user_id", "status", "end_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "ux_subscriptions_user_active",
                table: "subscriptions",
                column: "user_id",
                unique: true,
                filter: "\"status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
