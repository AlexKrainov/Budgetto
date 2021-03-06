﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _105 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_LimitNotifications_LimitNotificationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ReminderNotifications_ReminderNotificationID",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "LimitNotifications");

            migrationBuilder.DropTable(
                name: "ReminderNotifications");

            migrationBuilder.RenameColumn(
                name: "ReminderNotificationID",
                table: "Notifications",
                newName: "ReminderID");

            migrationBuilder.RenameColumn(
                name: "LimitNotificationID",
                table: "Notifications",
                newName: "LimitID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReminderNotificationID",
                table: "Notifications",
                newName: "IX_Notifications_ReminderID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_LimitNotificationID",
                table: "Notifications",
                newName: "IX_Notifications_LimitID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Limits_LimitID",
                table: "Notifications",
                column: "LimitID",
                principalTable: "Limits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Reminders_ReminderID",
                table: "Notifications",
                column: "ReminderID",
                principalTable: "Reminders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Limits_LimitID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Reminders_ReminderID",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ReminderID",
                table: "Notifications",
                newName: "ReminderNotificationID");

            migrationBuilder.RenameColumn(
                name: "LimitID",
                table: "Notifications",
                newName: "LimitNotificationID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReminderID",
                table: "Notifications",
                newName: "IX_Notifications_ReminderNotificationID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_LimitID",
                table: "Notifications",
                newName: "IX_Notifications_LimitNotificationID");

            migrationBuilder.CreateTable(
                name: "LimitNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimitID = table.Column<int>(nullable: false),
                    NotificationID = table.Column<int>(nullable: true)
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
    }
}
