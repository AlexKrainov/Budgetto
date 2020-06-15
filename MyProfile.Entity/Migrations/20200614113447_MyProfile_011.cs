using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GoalPage_IsShow_Collective",
                table: "UserSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GoalPage_IsShow_Finished",
                table: "UserSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalPage_IsShow_Collective",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "GoalPage_IsShow_Finished",
                table: "UserSettings");
        }
    }
}
