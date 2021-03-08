using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSent",
                table: "Notifications",
                newName: "IsSentOnTelegram");

            migrationBuilder.AddColumn<bool>(
                name: "IsSentOnMail",
                table: "Notifications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSentOnSite",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSentOnMail",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSentOnSite",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "IsSentOnTelegram",
                table: "Notifications",
                newName: "IsSent");
        }
    }
}
