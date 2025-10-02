using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrakkarVpn.Core.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTariffIdFromSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TariffId",
                table: "Subscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TariffId",
                table: "Subscriptions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
