using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateReminde",
                table: "Reminders",
                newName: "DateReminder");

            migrationBuilder.RenameColumn(
                name: "DateReminde",
                table: "ReminderDates",
                newName: "DateReminder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateReminder",
                table: "Reminders",
                newName: "DateReminde");

            migrationBuilder.RenameColumn(
                name: "DateReminder",
                table: "ReminderDates",
                newName: "DateReminde");
        }
    }
}
