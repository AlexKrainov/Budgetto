using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateReminde = table.Column<DateTime>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    IsRepeat = table.Column<bool>(nullable: false),
                    RepeatEvery = table.Column<int>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reminders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReminderDates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateReminde = table.Column<DateTime>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    ReminderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderDates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReminderDates_Reminders_ReminderID",
                        column: x => x.ReminderID,
                        principalTable: "Reminders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReminderDates_ReminderID",
                table: "ReminderDates",
                column: "ReminderID");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserID",
                table: "Reminders",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReminderDates");

            migrationBuilder.DropTable(
                name: "Reminders");
        }
    }
}
