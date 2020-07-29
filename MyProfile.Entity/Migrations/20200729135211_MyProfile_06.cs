using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_06 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CssColor",
                table: "BudgetSections",
                maxLength: 24,
                nullable: true,
                defaultValue: "#rgba(24,28,33,0.8)",
                oldClrType: typeof(string),
                oldMaxLength: 24,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CssBackground",
                table: "BudgetSections",
                maxLength: 24,
                nullable: true,
                defaultValue: "#eeeeee");

            migrationBuilder.AddColumn<string>(
                name: "CssBorder",
                table: "BudgetSections",
                maxLength: 24,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CssBackground",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "CssBorder",
                table: "BudgetSections");

            migrationBuilder.AlterColumn<string>(
                name: "CssColor",
                table: "BudgetSections",
                maxLength: 24,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 24,
                oldNullable: true,
                oldDefaultValue: "#rgba(24,28,33,0.8)");
        }
    }
}
