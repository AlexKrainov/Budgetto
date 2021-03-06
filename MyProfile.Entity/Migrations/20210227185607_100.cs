using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationTypeID = table.Column<int>(nullable: false),
                    IsReady = table.Column<bool>(nullable: false),
                    IsSent = table.Column<bool>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    LastChangeDateTime = table.Column<DateTime>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    ReadDateTime = table.Column<DateTime>(nullable: false),
                    IsSite = table.Column<bool>(nullable: false),
                    IsMail = table.Column<bool>(nullable: false),
                    IsTelegram = table.Column<bool>(nullable: false),
                    Total = table.Column<decimal>(type: "Money", nullable: true),
                    ExpirationDateTime = table.Column<DateTime>(nullable: false),
                    Icon = table.Column<string>(maxLength: 64, nullable: true),
                    UserID = table.Column<Guid>(nullable: false),
                    LimitNotificationID = table.Column<int>(nullable: true),
                    ReminderNotificationID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LimitNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationID = table.Column<int>(nullable: true),
                    LimitID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitNotifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LimitNotifications_Limits_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LimitNotifications_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReminderNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationID = table.Column<int>(nullable: true),
                    ReminderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderNotifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReminderNotifications_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReminderNotifications_Reminders_ReminderID",
                        column: x => x.ReminderID,
                        principalTable: "Reminders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LimitNotifications_LimitID",
                table: "LimitNotifications",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_LimitNotifications_NotificationID",
                table: "LimitNotifications",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LimitNotificationID",
                table: "Notifications",
                column: "LimitNotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReminderNotificationID",
                table: "Notifications",
                column: "ReminderNotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderNotifications_NotificationID",
                table: "ReminderNotifications",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderNotifications_ReminderID",
                table: "ReminderNotifications",
                column: "ReminderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_LimitNotifications_LimitNotificationID",
                table: "Notifications",
                column: "LimitNotificationID",
                principalTable: "LimitNotifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ReminderNotifications_ReminderNotificationID",
                table: "Notifications",
                column: "ReminderNotificationID",
                principalTable: "ReminderNotifications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LimitNotifications_Notifications_NotificationID",
                table: "LimitNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ReminderNotifications_Notifications_NotificationID",
                table: "ReminderNotifications");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "LimitNotifications");

            migrationBuilder.DropTable(
                name: "ReminderNotifications");
        }
    }
}
