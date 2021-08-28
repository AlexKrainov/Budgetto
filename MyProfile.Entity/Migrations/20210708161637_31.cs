using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mail_News",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Mail_Reminders",
                table: "UserSettings");

            migrationBuilder.AddColumn<int>(
                name: "SystemMailingID",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SystemMailings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    CodeName = table.Column<string>(maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CronExpression = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMailings", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SystemMailingID",
                table: "Notifications",
                column: "SystemMailingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_SystemMailings_SystemMailingID",
                table: "Notifications",
                column: "SystemMailingID",
                principalTable: "SystemMailings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_SystemMailings_SystemMailingID",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "SystemMailings");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SystemMailingID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SystemMailingID",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "Mail_News",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Mail_Reminders",
                table: "UserSettings",
                nullable: false,
                defaultValue: true);
        }
    }
}
