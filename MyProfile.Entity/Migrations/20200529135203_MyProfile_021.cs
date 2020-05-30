using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludedCollectiveSections",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "SectionTypeCodeName",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "IncludedCollectiveAreas",
                table: "BudgetAreas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncludedCollectiveSections",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectionTypeCodeName",
                table: "BudgetSections",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IncludedCollectiveAreas",
                table: "BudgetAreas",
                nullable: true);
        }
    }
}
