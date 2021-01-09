using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_072 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Month_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Month_Summary",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Year_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Year_Summary",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month_Accounts",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Month_Summary",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Year_Accounts",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Year_Summary",
                table: "UserSettings");
        }
    }
}
