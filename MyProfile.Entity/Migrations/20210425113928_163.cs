using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _163 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_PaymentTariff_PaymentTariffID",
                table: "PaymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentTariff_PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTariff",
                table: "PaymentTariff");

            migrationBuilder.RenameTable(
                name: "PaymentTariff",
                newName: "PaymentTariffs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTariffs",
                table: "PaymentTariffs",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_PaymentTariffs_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID",
                principalTable: "PaymentTariffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID",
                principalTable: "PaymentTariffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_PaymentTariffs_PaymentTariffID",
                table: "PaymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTariffs",
                table: "PaymentTariffs");

            migrationBuilder.RenameTable(
                name: "PaymentTariffs",
                newName: "PaymentTariff");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTariff",
                table: "PaymentTariff",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_PaymentTariff_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID",
                principalTable: "PaymentTariff",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentTariff_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID",
                principalTable: "PaymentTariff",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
