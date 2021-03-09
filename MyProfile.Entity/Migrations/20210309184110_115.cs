using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_TelegramAccount_TelegramAccountID",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TelegramAccount_TelegramAccountID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserID",
                table: "TelegramAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TelegramAccount");

            migrationBuilder.RenameTable(
                name: "TelegramAccount",
                newName: "TelegramAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_TelegramAccount_UserID",
                table: "TelegramAccounts",
                newName: "IX_TelegramAccounts_UserID");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "TelegramAccounts",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramAccounts",
                table: "TelegramAccounts",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "TelegramAccountStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true),
                    CodeName = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccountStatuses", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccounts_StatusID",
                table: "TelegramAccounts",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_TelegramAccounts_TelegramAccountID",
                table: "ChatUsers",
                column: "TelegramAccountID",
                principalTable: "TelegramAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TelegramAccounts_TelegramAccountID",
                table: "Notifications",
                column: "TelegramAccountID",
                principalTable: "TelegramAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccounts_TelegramAccountStatuses_StatusID",
                table: "TelegramAccounts",
                column: "StatusID",
                principalTable: "TelegramAccountStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccounts_UserConnects_UserID",
                table: "TelegramAccounts",
                column: "UserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_TelegramAccounts_TelegramAccountID",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TelegramAccounts_TelegramAccountID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccounts_TelegramAccountStatuses_StatusID",
                table: "TelegramAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_TelegramAccounts_UserConnects_UserID",
                table: "TelegramAccounts");

            migrationBuilder.DropTable(
                name: "TelegramAccountStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramAccounts",
                table: "TelegramAccounts");

            migrationBuilder.DropIndex(
                name: "IX_TelegramAccounts_StatusID",
                table: "TelegramAccounts");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "TelegramAccounts");

            migrationBuilder.RenameTable(
                name: "TelegramAccounts",
                newName: "TelegramAccount");

            migrationBuilder.RenameIndex(
                name: "IX_TelegramAccounts_UserID",
                table: "TelegramAccount",
                newName: "IX_TelegramAccount_UserID");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TelegramAccount",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_TelegramAccount_TelegramAccountID",
                table: "ChatUsers",
                column: "TelegramAccountID",
                principalTable: "TelegramAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TelegramAccount_TelegramAccountID",
                table: "Notifications",
                column: "TelegramAccountID",
                principalTable: "TelegramAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TelegramAccount_UserConnects_UserID",
                table: "TelegramAccount",
                column: "UserID",
                principalTable: "UserConnects",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
