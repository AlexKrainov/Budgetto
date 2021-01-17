using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _084 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyRateHistories_Currencies_CurrencyID",
                table: "CurrencyRateHistories");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyRateHistories_CurrencyID",
                table: "CurrencyRateHistories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRateHistories_CurrencyID",
                table: "CurrencyRateHistories",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyRateHistories_Currencies_CurrencyID",
                table: "CurrencyRateHistories",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
