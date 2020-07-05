using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_038 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "BudgetRecords",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyNominal",
                table: "BudgetRecords",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyRate",
                table: "BudgetRecords",
                type: "Money",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_CurrencyID",
                table: "BudgetRecords",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecords_Currencies_CurrencyID",
                table: "BudgetRecords",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetRecords_Currencies_CurrencyID",
                table: "BudgetRecords");

            migrationBuilder.DropIndex(
                name: "IX_BudgetRecords_CurrencyID",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "CurrencyNominal",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "BudgetRecords");
        }
    }
}
