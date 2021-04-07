using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _152 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "Limits",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Limits_CurrencyID",
                table: "Limits",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Limits_Currencies_CurrencyID",
                table: "Limits",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limits_Currencies_CurrencyID",
                table: "Limits");

            migrationBuilder.DropIndex(
                name: "IX_Limits_CurrencyID",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "Limits");
        }
    }
}
