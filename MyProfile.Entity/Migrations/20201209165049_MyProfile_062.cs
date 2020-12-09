using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewsLetter",
                table: "UserSettings",
                newName: "Mail_News");

            migrationBuilder.AddColumn<bool>(
                name: "CanUseAlgorithm",
                table: "UserSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Mail_Reminders",
                table: "UserSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanUseAlgorithm",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Mail_Reminders",
                table: "UserSettings");

            migrationBuilder.RenameColumn(
                name: "Mail_News",
                table: "UserSettings",
                newName: "NewsLetter");
        }
    }
}
