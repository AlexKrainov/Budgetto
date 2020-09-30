using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_042 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    Where = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ErrorText = table.Column<string>(nullable: true),
                    UserSessionID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_UserSessions_UserSessionID",
                        column: x => x.UserSessionID,
                        principalTable: "UserSessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_UserSessionID",
                table: "ErrorLogs",
                column: "UserSessionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    ErrorText = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    UserLogID = table.Column<int>(nullable: true),
                    UserSessionID = table.Column<Guid>(nullable: true),
                    Where = table.Column<string>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Logs_UserSessions_UserSessionID",
                        column: x => x.UserSessionID,
                        principalTable: "UserSessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserID",
                table: "Logs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserLogID",
                table: "Logs",
                column: "UserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserSessionID",
                table: "Logs",
                column: "UserSessionID");
        }
    }
}
