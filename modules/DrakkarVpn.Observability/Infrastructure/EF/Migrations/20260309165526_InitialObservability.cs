using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Observability.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialObservability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "observability");

            migrationBuilder.CreateTable(
                name: "core_alerts",
                schema: "observability",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DetailsJson = table.Column<string>(type: "jsonb", nullable: true),
                    resolution_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    resolution_note = table.Column<string>(type: "text", nullable: true),
                    resolved_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "core_error_events",
                schema: "observability",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Command = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ErrorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DomainCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    TraceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TelegramId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_error_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_CreatedAtUtc",
                schema: "observability",
                table: "core_alerts",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_IsResolved",
                schema: "observability",
                table: "core_alerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_IsResolved_CreatedAtUtc",
                schema: "observability",
                table: "core_alerts",
                columns: new[] { "IsResolved", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_resolved_by_admin_id",
                schema: "observability",
                table: "core_alerts",
                column: "resolved_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_ServerId",
                schema: "observability",
                table: "core_alerts",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_core_alerts_UserId",
                schema: "observability",
                table: "core_alerts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_alerts",
                schema: "observability");

            migrationBuilder.DropTable(
                name: "core_error_events",
                schema: "observability");
        }
    }
}
