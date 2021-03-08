using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectUserID",
                table: "TelegramAccount");

            migrationBuilder.RenameColumn(
                name: "UserConnectUserID",
                table: "TelegramAccount",
                newName: "UserConnectID");

            migrationBuilder.RenameIndex(
                name: "IX_TelegramAccount_UserConnectUserID",
                table: "TelegramAccount",
                newName: "IX_TelegramAccount_UserConnectID");

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectID",
                table: "TelegramAccount",
                column: "UserConnectID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectID",
                table: "TelegramAccount");

            migrationBuilder.RenameColumn(
                name: "UserConnectID",
                table: "TelegramAccount",
                newName: "UserConnectUserID");

            migrationBuilder.RenameIndex(
                name: "IX_TelegramAccount_UserConnectID",
                table: "TelegramAccount",
                newName: "IX_TelegramAccount_UserConnectUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserConnectUserID",
                table: "TelegramAccount",
                column: "UserConnectUserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
