using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_051 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhone",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTablet",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserVisible",
                table: "UserLogs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ObjectID",
                table: "UserLogs",
                maxLength: 40,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhone",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsTablet",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsUserVisible",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ObjectID",
                table: "UserLogs");
        }
    }
}
