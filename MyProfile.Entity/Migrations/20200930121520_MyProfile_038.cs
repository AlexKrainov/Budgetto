using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_038 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContinentCode",
                table: "UserSessions",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContinentName",
                table: "UserSessions",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Index",
                table: "UserSessions",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "UserSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "UserSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderInfo",
                table: "UserSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "UserSessions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Threat",
                table: "UserSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContinentCode",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ContinentName",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ProviderInfo",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Threat",
                table: "UserSessions");
        }
    }
}
