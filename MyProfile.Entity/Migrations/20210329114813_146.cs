using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _146 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountHistories_Accounts_AccountID2",
                table: "AccountHistories");

            migrationBuilder.RenameColumn(
                name: "AccountID2",
                table: "AccountHistories",
                newName: "AccountIDFrom");

            migrationBuilder.RenameIndex(
                name: "IX_AccountHistories_AccountID2",
                table: "AccountHistories",
                newName: "IX_AccountHistories_AccountIDFrom");

            migrationBuilder.AddColumn<string>(
                name: "StateField",
                table: "AccountHistories",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHistories_Accounts_AccountIDFrom",
                table: "AccountHistories",
                column: "AccountIDFrom",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountHistories_Accounts_AccountIDFrom",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "StateField",
                table: "AccountHistories");

            migrationBuilder.RenameColumn(
                name: "AccountIDFrom",
                table: "AccountHistories",
                newName: "AccountID2");

            migrationBuilder.RenameIndex(
                name: "IX_AccountHistories_AccountIDFrom",
                table: "AccountHistories",
                newName: "IX_AccountHistories_AccountID2");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHistories_Accounts_AccountID2",
                table: "AccountHistories",
                column: "AccountID2",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
