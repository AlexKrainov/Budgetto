using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _095 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_UserSessions_UserSessionID",
                table: "ErrorLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserSessionID",
                table: "ErrorLogs",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_UserSessions_UserSessionID",
                table: "ErrorLogs",
                column: "UserSessionID",
                principalTable: "UserSessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_UserSessions_UserSessionID",
                table: "ErrorLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserSessionID",
                table: "ErrorLogs",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_UserSessions_UserSessionID",
                table: "ErrorLogs",
                column: "UserSessionID",
                principalTable: "UserSessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
