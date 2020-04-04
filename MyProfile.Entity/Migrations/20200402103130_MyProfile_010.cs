using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetAreas");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "BudgetAreas");

            migrationBuilder.DropColumn(
                name: "CurrencyPrice",
                table: "BudgetAreas");

            migrationBuilder.RenameColumn(
                name: "FormatMoney",
                table: "PersonSettings",
                newName: "SpecificCulture");

            migrationBuilder.RenameColumn(
                name: "Type_RecordType",
                table: "BudgetSections",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "BudgetAreas",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "CssIcon",
                table: "BudgetSections",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CssColor",
                table: "BudgetSections",
                maxLength: 24,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CssIcon",
                table: "BudgetAreas",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CssColor",
                table: "BudgetSections");

            migrationBuilder.RenameColumn(
                name: "SpecificCulture",
                table: "PersonSettings",
                newName: "FormatMoney");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BudgetSections",
                newName: "Type_RecordType");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BudgetAreas",
                newName: "Currency");

            migrationBuilder.AlterColumn<string>(
                name: "CssIcon",
                table: "BudgetSections",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CssIcon",
                table: "BudgetAreas",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetAreas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "BudgetAreas",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyPrice",
                table: "BudgetAreas",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
