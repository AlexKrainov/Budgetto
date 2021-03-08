using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _114 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectID",
                table: "TelegramAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_Users_UserID",
                table: "TelegramAccount");

            migrationBuilder.DropIndex(
                name: "IX_TelegramAccount_UserConnectID",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "UserConnectID",
                table: "TelegramAccount");

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserID",
                table: "TelegramAccount",
                column: "UserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserID",
                table: "TelegramAccount");

            migrationBuilder.AddColumn<Guid>(
                name: "UserConnectID",
                table: "TelegramAccount",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_UserConnectID",
                table: "TelegramAccount",
                column: "UserConnectID");

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectID",
                table: "TelegramAccount",
                column: "UserConnectID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_Users_UserID",
                table: "TelegramAccount",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
