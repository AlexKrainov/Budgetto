using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_ReminderTypes_ReminderTypeID",
                table: "Reminders");

            migrationBuilder.DropTable(
                name: "ReminderTypes");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_ReminderTypeID",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ReminderTypeID",
                table: "Reminders");

            migrationBuilder.AddColumn<string>(
                name: "CssIcon",
                table: "Reminders",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CssIcon",
                table: "Reminders");

            migrationBuilder.AddColumn<int>(
                name: "ReminderTypeID",
                table: "Reminders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReminderTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    CssIcon = table.Column<string>(maxLength: 32, nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderTypes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_ReminderTypeID",
                table: "Reminders",
                column: "ReminderTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_ReminderTypes_ReminderTypeID",
                table: "Reminders",
                column: "ReminderTypeID",
                principalTable: "ReminderTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
