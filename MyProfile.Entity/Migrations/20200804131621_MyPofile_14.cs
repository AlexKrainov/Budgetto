using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyPofile_14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "ChatUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLeft",
                table: "ChatUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMute",
                table: "ChatUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "DateLeft",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "IsMute",
                table: "ChatUsers");
        }
    }
}
