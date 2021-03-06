using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnects_Users_ID",
                table: "UserConnects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectID",
                table: "UserConnects");

            migrationBuilder.RenameColumn(
                name: "UserConnectID",
                table: "UserConnects",
                newName: "UserConnectUserID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "UserConnects",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnects_UserConnectID",
                table: "UserConnects",
                newName: "IX_UserConnects_UserConnectUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectUserID",
                table: "UserConnects",
                column: "UserConnectUserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnects_Users_UserID",
                table: "UserConnects",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectUserID",
                table: "UserConnects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConnects_Users_UserID",
                table: "UserConnects");

            migrationBuilder.RenameColumn(
                name: "UserConnectUserID",
                table: "UserConnects",
                newName: "UserConnectID");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserConnects",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnects_UserConnectUserID",
                table: "UserConnects",
                newName: "IX_UserConnects_UserConnectID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnects_Users_ID",
                table: "UserConnects",
                column: "ID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectID",
                table: "UserConnects",
                column: "UserConnectID",
                principalTable: "UserConnects",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
