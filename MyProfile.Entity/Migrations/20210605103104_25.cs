using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetSections");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BudgetSections",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BudgetSections",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BudgetAreas",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "BaseAreaID",
                table: "BudgetAreas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAreas_BaseAreaID",
                table: "BudgetAreas",
                column: "BaseAreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAreas_BaseAreas_BaseAreaID",
                table: "BudgetAreas",
                column: "BaseAreaID",
                principalTable: "BaseAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAreas_BaseAreas_BaseAreaID",
                table: "BudgetAreas");

            migrationBuilder.DropIndex(
                name: "IX_BudgetAreas_BaseAreaID",
                table: "BudgetAreas");

            migrationBuilder.DropColumn(
                name: "BaseAreaID",
                table: "BudgetAreas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BudgetSections",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BudgetSections",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BudgetAreas",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);
        }
    }
}
