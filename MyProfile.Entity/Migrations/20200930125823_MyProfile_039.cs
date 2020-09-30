using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_039 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentDateTime",
                table: "UserSessions",
                newName: "EnterDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogOutDate",
                table: "UserSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogOutDate",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "EnterDate",
                table: "UserSessions",
                newName: "CurrentDateTime");
        }
    }
}
