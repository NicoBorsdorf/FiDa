using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiDa.Migrations
{
    /// <inheritdoc />
    public partial class NewPro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserHost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Host = table.Column<int>(type: "int", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHost_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HostId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ParentFolderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsFolder = table.Column<bool>(type: "bit", nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UploadedFiles_UserHost_HostId",
                        column: x => x.HostId,
                        principalTable: "UserHost",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Id_Username",
                table: "Account",
                columns: new[] { "Id", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_AccountId",
                table: "UploadedFiles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_HostId",
                table: "UploadedFiles",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Id",
                table: "UploadedFiles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserHost_AccountId",
                table: "UserHost",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHost_Id_AccountId_Host",
                table: "UserHost",
                columns: new[] { "Id", "AccountId", "Host" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "UserHost");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
