using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddServerTransportProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "server_transport_profiles",
                schema: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    transport_type = table.Column<int>(type: "integer", nullable: false),
                    security_type = table.Column<int>(type: "integer", nullable: false),
                    reality_sni = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    reality_short_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    reality_fingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reality_public_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    reality_dest = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    grpc_service_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grpc_authority = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    activated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_transport_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_server_transport_profiles_servers_server_id",
                        column: x => x.server_id,
                        principalSchema: "servers",
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_server_transport_profiles_one_active_per_server",
                schema: "servers",
                table: "server_transport_profiles",
                column: "server_id",
                unique: true,
                filter: "status = 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_transport_profiles",
                schema: "servers");
        }
    }
}
