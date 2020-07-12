using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_041 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LimitPage_Show_IsFinished",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowInCollective",
                table: "Limits",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitPage_Show_IsFinished",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "IsShowInCollective",
                table: "Limits");
        }
    }
}
