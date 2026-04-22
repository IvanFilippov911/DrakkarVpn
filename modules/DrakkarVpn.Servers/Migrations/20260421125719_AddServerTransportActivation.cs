using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Servers.Migrations
{
    /// <inheritdoc />
    public partial class AddServerTransportActivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_transport_profiles",
                schema: "servers");

            migrationBuilder.CreateTable(
                name: "transport_profiles",
                schema: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    transport_type = table.Column<int>(type: "integer", nullable: false),
                    security_type = table.Column<int>(type: "integer", nullable: false),
                    reality_sni = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    reality_short_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    reality_fingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reality_dest = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    grpc_service_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grpc_authority = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    global_priority = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transport_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server_transport_activations",
                schema: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transport_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reality_public_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    local_priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    activated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_transport_activations", x => x.id);
                    table.ForeignKey(
                        name: "FK_server_transport_activations_servers_server_id",
                        column: x => x.server_id,
                        principalSchema: "servers",
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_server_transport_activations_transport_profiles_transport_p~",
                        column: x => x.transport_profile_id,
                        principalSchema: "servers",
                        principalTable: "transport_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_server_transport_activations_server_id_local_priority",
                schema: "servers",
                table: "server_transport_activations",
                columns: new[] { "server_id", "local_priority" });

            migrationBuilder.CreateIndex(
                name: "IX_server_transport_activations_transport_profile_id",
                schema: "servers",
                table: "server_transport_activations",
                column: "transport_profile_id");

            migrationBuilder.CreateIndex(
                name: "ux_server_transport_activations_one_active_per_server",
                schema: "servers",
                table: "server_transport_activations",
                column: "server_id",
                unique: true,
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ux_server_transport_activations_server_profile",
                schema: "servers",
                table: "server_transport_activations",
                columns: new[] { "server_id", "transport_profile_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transport_profiles_enabled_priority",
                schema: "servers",
                table: "transport_profiles",
                columns: new[] { "is_enabled", "global_priority" });

            migrationBuilder.CreateIndex(
                name: "ix_transport_profiles_name",
                schema: "servers",
                table: "transport_profiles",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_transport_activations",
                schema: "servers");

            migrationBuilder.DropTable(
                name: "transport_profiles",
                schema: "servers");

            migrationBuilder.CreateTable(
                name: "server_transport_profiles",
                schema: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    activated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    grpc_authority = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    grpc_service_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    reality_dest = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    reality_fingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reality_public_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    reality_short_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    reality_sni = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    security_type = table.Column<int>(type: "integer", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    transport_type = table.Column<int>(type: "integer", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
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
                name: "ix_server_transport_profiles_server_id_priority",
                schema: "servers",
                table: "server_transport_profiles",
                columns: new[] { "server_id", "priority" });

            migrationBuilder.CreateIndex(
                name: "ux_server_transport_profiles_one_active_per_server",
                schema: "servers",
                table: "server_transport_profiles",
                column: "server_id",
                unique: true,
                filter: "status = 2");
        }
    }
}
