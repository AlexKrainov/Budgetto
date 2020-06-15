using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                table: "SectionGroupLimits");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetSectionID",
                table: "SectionGroupLimits",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                table: "SectionGroupLimits",
                column: "BudgetSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                table: "SectionGroupLimits");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetSectionID",
                table: "SectionGroupLimits",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                table: "SectionGroupLimits",
                column: "BudgetSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
