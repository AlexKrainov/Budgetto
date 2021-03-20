using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _126 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBankTypes_AccountTypes_BankTypeID",
                table: "AccountBankTypes");

            migrationBuilder.RenameColumn(
                name: "BankTypeID",
                table: "AccountBankTypes",
                newName: "AccountTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountBankTypes_BankTypeID",
                table: "AccountBankTypes",
                newName: "IX_AccountBankTypes_AccountTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBankTypes_AccountTypes_AccountTypeID",
                table: "AccountBankTypes",
                column: "AccountTypeID",
                principalTable: "AccountTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBankTypes_AccountTypes_AccountTypeID",
                table: "AccountBankTypes");

            migrationBuilder.RenameColumn(
                name: "AccountTypeID",
                table: "AccountBankTypes",
                newName: "BankTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountBankTypes_AccountTypeID",
                table: "AccountBankTypes",
                newName: "IX_AccountBankTypes_BankTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBankTypes_AccountTypes_BankTypeID",
                table: "AccountBankTypes",
                column: "BankTypeID",
                principalTable: "AccountTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
