using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _135 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountHistories_AccountID2",
                table: "AccountHistories",
                column: "AccountID2");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHistories_Accounts_AccountID2",
                table: "AccountHistories",
                column: "AccountID2",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountHistories_Accounts_AccountID2",
                table: "AccountHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccountHistories_AccountID2",
                table: "AccountHistories");
        }
    }
}
