using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_034 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "Users",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrencyID",
                table: "Users",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Currencies_CurrencyID",
                table: "Users",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Currencies_CurrencyID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurrencyID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "Users");
        }
    }
}
