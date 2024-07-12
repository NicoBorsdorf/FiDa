using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiDa.Migrations
{
    /// <inheritdoc />
    public partial class AppSecretProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppSecret",
                table: "UserHost",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppSecret",
                table: "UserHost");
        }
    }
}
