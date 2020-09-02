using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_029 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerID",
                table: "HelpArticles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticles_OwnerID",
                table: "HelpArticles",
                column: "OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_Users_OwnerID",
                table: "HelpArticles",
                column: "OwnerID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_Users_OwnerID",
                table: "HelpArticles");

            migrationBuilder.DropIndex(
                name: "IX_HelpArticles_OwnerID",
                table: "HelpArticles");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "HelpArticles");
        }
    }
}
