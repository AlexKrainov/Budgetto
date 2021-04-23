using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _160 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubScriptionPricings_Users_UserID",
                table: "SubScriptionPricings");

            migrationBuilder.DropIndex(
                name: "IX_SubScriptionPricings_UserID",
                table: "SubScriptionPricings");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "SubScriptionPricings");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "UserSubScriptions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubScriptions_UserID",
                table: "UserSubScriptions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubScriptions_UserID",
                table: "UserSubScriptions");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserSubScriptions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "SubScriptionPricings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptionPricings_UserID",
                table: "SubScriptionPricings",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubScriptionPricings_Users_UserID",
                table: "SubScriptionPricings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
