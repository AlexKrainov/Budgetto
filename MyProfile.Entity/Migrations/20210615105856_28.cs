using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections");

            migrationBuilder.AlterColumn<int>(
                name: "SectionTypeID",
                table: "BudgetSections",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID",
                principalTable: "SectionTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections");

            migrationBuilder.AlterColumn<int>(
                name: "SectionTypeID",
                table: "BudgetSections",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID",
                principalTable: "SectionTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
