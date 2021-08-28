using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMail",
                table: "SystemMailings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSite",
                table: "SystemMailings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTelegram",
                table: "SystemMailings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMail",
                table: "SystemMailings");

            migrationBuilder.DropColumn(
                name: "IsSite",
                table: "SystemMailings");

            migrationBuilder.DropColumn(
                name: "IsTelegram",
                table: "SystemMailings");
        }
    }
}
