using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limits_BudgetSections_BudgetSectionID",
                table: "Limits");

            migrationBuilder.DropIndex(
                name: "IX_Limits_BudgetSectionID",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "BudgetSectionID",
                table: "Limits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetSectionID",
                table: "Limits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetSectionID",
                table: "Limits",
                column: "BudgetSectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Limits_BudgetSections_BudgetSectionID",
                table: "Limits",
                column: "BudgetSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
