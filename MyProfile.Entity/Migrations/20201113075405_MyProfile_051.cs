using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_051 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByConstructor",
                table: "Templates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByConstructor",
                table: "Limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByConstructor",
                table: "Goals",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByConstructor",
                table: "BudgetSections",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByConstructor",
                table: "BudgetAreas",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreatedByConstructor",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "IsCreatedByConstructor",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "IsCreatedByConstructor",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsCreatedByConstructor",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "IsCreatedByConstructor",
                table: "BudgetAreas");
        }
    }
}
