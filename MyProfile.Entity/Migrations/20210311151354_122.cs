using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Reminders_ReminderID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ReminderID",
                table: "Notifications",
                newName: "ReminderDateID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReminderID",
                table: "Notifications",
                newName: "IX_Notifications_ReminderDateID");

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeat",
                table: "Notifications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ReminderDates_ReminderDateID",
                table: "Notifications",
                column: "ReminderDateID",
                principalTable: "ReminderDates",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ReminderDates_ReminderDateID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsRepeat",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ReminderDateID",
                table: "Notifications",
                newName: "ReminderID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReminderDateID",
                table: "Notifications",
                newName: "IX_Notifications_ReminderID");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Notifications",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Reminders_ReminderID",
                table: "Notifications",
                column: "ReminderID",
                principalTable: "Reminders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
