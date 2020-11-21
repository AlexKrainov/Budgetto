using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_055 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserVisible",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ObjectID",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "SessionID",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "SessionID",
                table: "IPSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUserVisible",
                table: "UserSessions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ObjectID",
                table: "UserSessions",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "UserSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "UserSessions",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionID",
                table: "UserSessions",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionID",
                table: "IPSettings",
                maxLength: 32,
                nullable: true);
        }
    }
}
