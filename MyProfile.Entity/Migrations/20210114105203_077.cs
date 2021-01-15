using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _077 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewAccountBalance",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewAccountCashback",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewRecordCachback",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewRecordTotal",
                table: "AccountRecordHistories");

            migrationBuilder.RenameColumn(
                name: "OldRecordTotal",
                table: "AccountRecordHistories",
                newName: "RecordTotal");

            migrationBuilder.RenameColumn(
                name: "OldRecordCachback",
                table: "AccountRecordHistories",
                newName: "RecordCachback");

            migrationBuilder.RenameColumn(
                name: "OldAccountCashback",
                table: "AccountRecordHistories",
                newName: "AccountCashback");

            migrationBuilder.RenameColumn(
                name: "OldAccountBalance",
                table: "AccountRecordHistories",
                newName: "AccountBalance");

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                table: "AccountRecordHistories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RecordCurrencyID",
                table: "AccountRecordHistories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "AccountCurrencyID",
                table: "AccountRecordHistories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_AccountCurrencyID",
                table: "AccountRecordHistories",
                column: "AccountCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_RecordCurrencyID",
                table: "AccountRecordHistories",
                column: "RecordCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_SectionID",
                table: "AccountRecordHistories",
                column: "SectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRecordHistories_Currencies_AccountCurrencyID",
                table: "AccountRecordHistories",
                column: "AccountCurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRecordHistories_Currencies_RecordCurrencyID",
                table: "AccountRecordHistories",
                column: "RecordCurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRecordHistories_BudgetSections_SectionID",
                table: "AccountRecordHistories",
                column: "SectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRecordHistories_Currencies_AccountCurrencyID",
                table: "AccountRecordHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRecordHistories_Currencies_RecordCurrencyID",
                table: "AccountRecordHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRecordHistories_BudgetSections_SectionID",
                table: "AccountRecordHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccountRecordHistories_AccountCurrencyID",
                table: "AccountRecordHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccountRecordHistories_RecordCurrencyID",
                table: "AccountRecordHistories");

            migrationBuilder.DropIndex(
                name: "IX_AccountRecordHistories_SectionID",
                table: "AccountRecordHistories");

            migrationBuilder.RenameColumn(
                name: "RecordTotal",
                table: "AccountRecordHistories",
                newName: "OldRecordTotal");

            migrationBuilder.RenameColumn(
                name: "RecordCachback",
                table: "AccountRecordHistories",
                newName: "OldRecordCachback");

            migrationBuilder.RenameColumn(
                name: "AccountCashback",
                table: "AccountRecordHistories",
                newName: "OldAccountCashback");

            migrationBuilder.RenameColumn(
                name: "AccountBalance",
                table: "AccountRecordHistories",
                newName: "OldAccountBalance");

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                table: "AccountRecordHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecordCurrencyID",
                table: "AccountRecordHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountCurrencyID",
                table: "AccountRecordHistories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewAccountBalance",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewAccountCashback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewRecordCachback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewRecordTotal",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
