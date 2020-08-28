using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_026 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_UserLogs_ParentUserLogID",
                table: "UserLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_Users_UserID",
                table: "UserLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserLogs_ParentUserLogID",
                table: "UserLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserLogs_UserID",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "BrowerName",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "BrowserVersion",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IP",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsPhone",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsTablet",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsUserVisible",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "OS_Name",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ObjectID",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Os_Version",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ParentUserLogID",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ScreenSize",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "SessionID",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserLogs");

            migrationBuilder.AddColumn<Guid>(
                name: "UserSessionID",
                table: "UserLogs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    Where = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ErrorText = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    UserLogID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_UserLogs_UserLogID",
                        column: x => x.UserLogID,
                        principalTable: "UserLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IP = table.Column<string>(maxLength: 64, nullable: true),
                    City = table.Column<string>(maxLength: 32, nullable: true),
                    Country = table.Column<string>(maxLength: 32, nullable: true),
                    Location = table.Column<string>(maxLength: 64, nullable: true),
                    PostCode = table.Column<string>(maxLength: 16, nullable: true),
                    BrowerName = table.Column<string>(maxLength: 32, nullable: true),
                    BrowserVersion = table.Column<string>(maxLength: 16, nullable: true),
                    OS_Name = table.Column<string>(maxLength: 32, nullable: true),
                    Os_Version = table.Column<string>(maxLength: 16, nullable: true),
                    ScreenSize = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    SessionID = table.Column<string>(maxLength: 32, nullable: true),
                    ObjectID = table.Column<string>(maxLength: 40, nullable: true),
                    IsUserVisible = table.Column<bool>(nullable: false),
                    IsPhone = table.Column<bool>(nullable: false),
                    IsTablet = table.Column<bool>(nullable: false),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserSessionID",
                table: "UserLogs",
                column: "UserSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserID",
                table: "Logs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserLogID",
                table: "Logs",
                column: "UserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserID",
                table: "UserSessions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_UserSessions_UserSessionID",
                table: "UserLogs",
                column: "UserSessionID",
                principalTable: "UserSessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogs_UserSessions_UserSessionID",
                table: "UserLogs");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserLogs_UserSessionID",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "UserSessionID",
                table: "UserLogs");

            migrationBuilder.AddColumn<string>(
                name: "BrowerName",
                table: "UserLogs",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrowserVersion",
                table: "UserLogs",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserLogs",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserLogs",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "UserLogs",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhone",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTablet",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserVisible",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "UserLogs",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OS_Name",
                table: "UserLogs",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectID",
                table: "UserLogs",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Os_Version",
                table: "UserLogs",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentUserLogID",
                table: "UserLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "UserLogs",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenSize",
                table: "UserLogs",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionID",
                table: "UserLogs",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "UserLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_ParentUserLogID",
                table: "UserLogs",
                column: "ParentUserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserID",
                table: "UserLogs",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_UserLogs_ParentUserLogID",
                table: "UserLogs",
                column: "ParentUserLogID",
                principalTable: "UserLogs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogs_Users_UserID",
                table: "UserLogs",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
