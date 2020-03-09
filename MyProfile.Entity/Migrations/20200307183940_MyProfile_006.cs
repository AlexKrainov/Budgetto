using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnType",
                table: "TemplateColumns");

            migrationBuilder.DropColumn(
                name: "FooterActionType",
                table: "TemplateColumns");

            migrationBuilder.AddColumn<int>(
                name: "ColumnTypeID",
                table: "TemplateColumns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FooterActionTypeID",
                table: "TemplateColumns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnTypeID",
                table: "TemplateColumns");

            migrationBuilder.DropColumn(
                name: "FooterActionTypeID",
                table: "TemplateColumns");

            migrationBuilder.AddColumn<string>(
                name: "ColumnType",
                table: "TemplateColumns",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterActionType",
                table: "TemplateColumns",
                nullable: true);
        }
    }
}
