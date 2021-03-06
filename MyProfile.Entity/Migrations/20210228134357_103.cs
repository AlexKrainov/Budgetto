using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectUserID",
                table: "UserConnects");

            migrationBuilder.DropIndex(
                name: "IX_UserConnects_UserConnectUserID",
                table: "UserConnects");

            migrationBuilder.DropColumn(
                name: "UserConnectUserID",
                table: "UserConnects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserConnectUserID",
                table: "UserConnects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConnects_UserConnectUserID",
                table: "UserConnects",
                column: "UserConnectUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnects_UserConnects_UserConnectUserID",
                table: "UserConnects",
                column: "UserConnectUserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
