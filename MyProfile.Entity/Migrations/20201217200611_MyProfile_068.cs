using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_068 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "UserTags",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_UserID",
                table: "UserTags",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTags_Users_UserID",
                table: "UserTags",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTags_Users_UserID",
                table: "UserTags");

            migrationBuilder.DropIndex(
                name: "IX_UserTags_UserID",
                table: "UserTags");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserTags");
        }
    }
}
