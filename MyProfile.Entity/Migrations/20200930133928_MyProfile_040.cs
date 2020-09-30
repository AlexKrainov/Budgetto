using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_040 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
