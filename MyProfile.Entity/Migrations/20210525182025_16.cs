using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProgressLogs",
                newName: "CurrentValue");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Progresses",
                newName: "NeedToBeValue");

            migrationBuilder.AddColumn<string>(
                name: "NeedToBeValue",
                table: "ProgressLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentValue",
                table: "Progresses",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedToBeValue",
                table: "ProgressLogs");

            migrationBuilder.DropColumn(
                name: "CurrentValue",
                table: "Progresses");

            migrationBuilder.RenameColumn(
                name: "CurrentValue",
                table: "ProgressLogs",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "NeedToBeValue",
                table: "Progresses",
                newName: "Value");
        }
    }
}
