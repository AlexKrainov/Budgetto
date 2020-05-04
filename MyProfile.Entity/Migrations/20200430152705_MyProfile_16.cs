using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsByDefault",
                table: "BudgetSections",
                newName: "IsShow");

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetAreas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetAreas");

            migrationBuilder.RenameColumn(
                name: "IsShow",
                table: "BudgetSections",
                newName: "IsByDefault");
        }
    }
}
