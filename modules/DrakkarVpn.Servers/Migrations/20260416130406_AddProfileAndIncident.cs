using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAndIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "transport_incidents",
                schema: "servers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActiveProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    TriggeredBy = table.Column<int>(type: "integer", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RemediationStartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RemediationFinishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EscalatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuccessfulProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    AffectedProbeCount = table.Column<int>(type: "integer", nullable: false),
                    FailedProbeCount = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transport_incidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "transport_incident_attempts",
                schema: "servers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transport_incident_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transport_incident_attempts_transport_incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalSchema: "servers",
                        principalTable: "transport_incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transport_incident_attempts_IncidentId",
                schema: "servers",
                table: "transport_incident_attempts",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_transport_incident_attempts_IncidentId_ProfileId",
                schema: "servers",
                table: "transport_incident_attempts",
                columns: new[] { "IncidentId", "ProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transport_incidents_ServerId",
                schema: "servers",
                table: "transport_incidents",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_transport_incidents_ServerId_Status",
                schema: "servers",
                table: "transport_incidents",
                columns: new[] { "ServerId", "Status" },
                unique: true,
                filter: "\"Status\" IN (1,2,3)");

            migrationBuilder.CreateIndex(
                name: "IX_transport_incidents_Status",
                schema: "servers",
                table: "transport_incidents",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transport_incident_attempts",
                schema: "servers");

            migrationBuilder.DropTable(
                name: "transport_incidents",
                schema: "servers");

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
    }
}
