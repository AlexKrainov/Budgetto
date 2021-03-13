using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_TimeZones_MyTimeZoneID",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_MyTimeZoneID",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "MyTimeZoneID",
                table: "Reminders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
