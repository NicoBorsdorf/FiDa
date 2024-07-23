using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiDa.Migrations
{
    /// <inheritdoc />
    public partial class OnCreateUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Account_AccountId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_UserHost_HostId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHost_Account_AccountId",
                table: "UserHost");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "UserHost",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "UserHost",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UriString",
                table: "UserHost",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "UploadedFiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "UploadedFiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Account",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Account",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Account",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_UserHost_Id",
                table: "UserHost",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Account_AccountId",
                table: "UploadedFiles",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_UserHost_HostId",
                table: "UploadedFiles",
                column: "HostId",
                principalTable: "UserHost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserHost_Account_AccountId",
                table: "UserHost",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Account_AccountId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_UserHost_HostId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHost_Account_AccountId",
                table: "UserHost");

            migrationBuilder.DropIndex(
                name: "IX_UserHost_Id",
                table: "UserHost");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "UserHost");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "UserHost");

            migrationBuilder.DropColumn(
                name: "UriString",
                table: "UserHost");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Account");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "UploadedFiles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "UploadedFiles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Account",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Account_AccountId",
                table: "UploadedFiles",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_UserHost_HostId",
                table: "UploadedFiles",
                column: "HostId",
                principalTable: "UserHost",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHost_Account_AccountId",
                table: "UserHost",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}
