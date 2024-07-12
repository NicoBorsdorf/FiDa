using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiDa.Migrations
{
    /// <inheritdoc />
    public partial class NewAppProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppKey",
                table: "UserHost",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppKey",
                table: "UserHost");
        }
    }
}
