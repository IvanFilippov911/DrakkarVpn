using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPeersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "peers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_peer_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    config_raw = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_peers_server_id",
                table: "peers",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "ix_peers_user_id",
                table: "peers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_peers_agent_peer_uuid",
                table: "peers",
                column: "agent_peer_uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peers");
        }
    }
}
