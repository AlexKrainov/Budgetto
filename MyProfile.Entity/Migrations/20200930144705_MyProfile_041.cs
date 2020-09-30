using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_041 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserSessionID",
                table: "Logs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserSessionID",
                table: "Logs",
                column: "UserSessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_UserSessions_UserSessionID",
                table: "Logs",
                column: "UserSessionID",
                principalTable: "UserSessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_UserSessions_UserSessionID",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_UserSessionID",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UserSessionID",
                table: "Logs");
        }
    }
}
