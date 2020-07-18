using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_044 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDelete",
                table: "Templates",
                newName: "IsDeleted");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetYear",
                table: "VisibleElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetMonth",
                table: "VisibleElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Templates",
                newName: "IsDelete");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetYear",
                table: "VisibleElements",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetMonth",
                table: "VisibleElements",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));
        }
    }
}
