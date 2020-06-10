using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BudgetPages_EarningChart",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "BudgetPages_InvestingChart",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "BudgetPages_SpendingChart",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetPages_EarningChart",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "BudgetPages_InvestingChart",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "BudgetPages_SpendingChart",
                table: "UserSettings");
        }
    }
}
