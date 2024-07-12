using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiDa.Migrations
{
    /// <inheritdoc />
    public partial class NewExpireProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiration",
                table: "UserHost",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenExpiration",
                table: "UserHost");
        }
    }
}
