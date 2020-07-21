using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_046 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShow",
                table: "BudgetSections",
                newName: "IsShowOnSite");

            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "BudgetSections",
                newName: "IsShowInCollective");

            migrationBuilder.RenameColumn(
                name: "IsShow",
                table: "BudgetAreas",
                newName: "IsShowOnSite");

            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "BudgetAreas",
                newName: "IsShowInCollective");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShowOnSite",
                table: "BudgetSections",
                newName: "IsShow");

            migrationBuilder.RenameColumn(
                name: "IsShowInCollective",
                table: "BudgetSections",
                newName: "IsPrivate");

            migrationBuilder.RenameColumn(
                name: "IsShowOnSite",
                table: "BudgetAreas",
                newName: "IsShow");

            migrationBuilder.RenameColumn(
                name: "IsShowInCollective",
                table: "BudgetAreas",
                newName: "IsPrivate");
        }
    }
}
