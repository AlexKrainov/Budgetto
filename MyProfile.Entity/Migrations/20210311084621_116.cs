using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _116 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReminderByMail",
                table: "Reminders");

            migrationBuilder.AddColumn<int>(
                name: "OffSet",
                table: "Reminders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Reminders",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffSet",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Reminders");

            migrationBuilder.AddColumn<bool>(
                name: "IsReminderByMail",
                table: "Reminders",
                nullable: false,
                defaultValue: false);
        }
    }
}
