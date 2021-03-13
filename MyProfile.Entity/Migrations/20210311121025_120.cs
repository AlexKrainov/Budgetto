using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_TimeZones_TimeZoneID",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "TimeZoneID",
                table: "Reminders",
                newName: "OlsonTZID");

            migrationBuilder.RenameIndex(
                name: "IX_Reminders_TimeZoneID",
                table: "Reminders",
                newName: "IX_Reminders_OlsonTZID");

            migrationBuilder.AddColumn<int>(
                name: "MyTimeZoneID",
                table: "Reminders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_MyTimeZoneID",
                table: "Reminders",
                column: "MyTimeZoneID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_TimeZones_MyTimeZoneID",
                table: "Reminders",
                column: "MyTimeZoneID",
                principalTable: "TimeZones",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_OlsonTZIDs_OlsonTZID",
                table: "Reminders",
                column: "OlsonTZID",
                principalTable: "OlsonTZIDs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_TimeZones_MyTimeZoneID",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_OlsonTZIDs_OlsonTZID",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_MyTimeZoneID",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "MyTimeZoneID",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "OlsonTZID",
                table: "Reminders",
                newName: "TimeZoneID");

            migrationBuilder.RenameIndex(
                name: "IX_Reminders_OlsonTZID",
                table: "Reminders",
                newName: "IX_Reminders_TimeZoneID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_TimeZones_TimeZoneID",
                table: "Reminders",
                column: "TimeZoneID",
                principalTable: "TimeZones",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
