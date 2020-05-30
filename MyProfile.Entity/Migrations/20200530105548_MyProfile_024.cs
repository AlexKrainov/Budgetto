using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_024 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                table: "CollectiveSections");

            migrationBuilder.AlterColumn<int>(
                name: "ChildSectionID",
                table: "CollectiveSections",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "SectionID",
                table: "CollectiveSections",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveSections_SectionID",
                table: "CollectiveSections",
                column: "SectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                table: "CollectiveSections",
                column: "ChildSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveSections_BudgetSections_SectionID",
                table: "CollectiveSections",
                column: "SectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                table: "CollectiveSections");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveSections_BudgetSections_SectionID",
                table: "CollectiveSections");

            migrationBuilder.DropIndex(
                name: "IX_CollectiveSections_SectionID",
                table: "CollectiveSections");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "CollectiveSections");

            migrationBuilder.AlterColumn<int>(
                name: "ChildSectionID",
                table: "CollectiveSections",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                table: "CollectiveSections",
                column: "ChildSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
