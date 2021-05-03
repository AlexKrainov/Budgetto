using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _166 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments");

            migrationBuilder.AddColumn<long>(
                name: "ID2",
                table: "UserLogs",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "Payments",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID",
                principalTable: "PaymentTariffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ID2",
                table: "UserLogs");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "Payments",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID",
                principalTable: "PaymentTariffs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
