using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "TelegramKey",
                table: "UserConnects");

            migrationBuilder.AddColumn<int>(
                name: "TelegramAccountID",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "ChatUsers",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "TelegramAccountID",
                table: "ChatUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatType",
                table: "Chats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TelegramAccount",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 32, nullable: true),
                    TelegramID = table.Column<int>(nullable: false),
                    Username = table.Column<string>(maxLength: 512, nullable: true),
                    FirstName = table.Column<string>(maxLength: 512, nullable: true),
                    LastName = table.Column<string>(maxLength: 512, nullable: true),
                    Title = table.Column<string>(maxLength: 512, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    LastDateConnect = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    UserConnectUserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccount", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelegramAccount_UserConnects_UserConnectUserID",
                        column: x => x.UserConnectUserID,
                        principalTable: "UserConnects",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TelegramAccount_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TelegramAccountID",
                table: "Notifications",
                column: "TelegramAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_TelegramAccountID",
                table: "ChatUsers",
                column: "TelegramAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_UserConnectUserID",
                table: "TelegramAccount",
                column: "UserConnectUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_UserID",
                table: "TelegramAccount",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_TelegramAccount_TelegramAccountID",
                table: "ChatUsers",
                column: "TelegramAccountID",
                principalTable: "TelegramAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TelegramAccount_TelegramAccountID",
                table: "Notifications",
                column: "TelegramAccountID",
                principalTable: "TelegramAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_TelegramAccount_TelegramAccountID",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TelegramAccount_TelegramAccountID",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "TelegramAccount");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TelegramAccountID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_TelegramAccountID",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "TelegramAccountID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TelegramAccountID",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "ChatType",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "TelegramKey",
                table: "UserConnects",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "ChatUsers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
