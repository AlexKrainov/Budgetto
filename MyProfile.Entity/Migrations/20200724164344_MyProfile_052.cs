using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_052 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Limits");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Limits",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Limits");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Limits",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "Limits",
                nullable: true);
        }
    }
}
