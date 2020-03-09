using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeCodeName",
                table: "TemplateColumns",
                newName: "ColumnType");

            migrationBuilder.AddColumn<string>(
                name: "FooterActionType",
                table: "TemplateColumns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterActionType",
                table: "TemplateColumns");

            migrationBuilder.RenameColumn(
                name: "ColumnType",
                table: "TemplateColumns",
                newName: "TypeCodeName");
        }
    }
}
