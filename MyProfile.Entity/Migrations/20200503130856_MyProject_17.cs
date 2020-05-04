using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProject_17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncludedCollectiveSections",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncludedCollectiveAreas",
                table: "BudgetAreas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                table: "BudgetAreas",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludedCollectiveSections",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "IncludedCollectiveAreas",
                table: "BudgetAreas");

            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "BudgetAreas");
        }
    }
}
