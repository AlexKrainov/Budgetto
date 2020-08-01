using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashPassword",
                table: "Users",
                maxLength: 44,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SaltPassword",
                table: "Users",
                maxLength: 44,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SaltPassword",
                table: "Users");
        }
    }
}
