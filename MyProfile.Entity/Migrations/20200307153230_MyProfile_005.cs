using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceAfterCommon",
                table: "TemplateColumns",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsByDefault",
                table: "BudgetSections",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceAfterCommon",
                table: "TemplateColumns");

            migrationBuilder.DropColumn(
                name: "IsByDefault",
                table: "BudgetSections");
        }
    }
}
