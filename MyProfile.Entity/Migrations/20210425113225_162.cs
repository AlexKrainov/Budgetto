using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _162 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tariff",
                table: "Payments",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTariffID",
                table: "Payments",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tariff",
                table: "PaymentHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTariffID",
                table: "PaymentHistories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentTariff",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: true),
                    CodeName = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTariff", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_PaymentTariff_PaymentTariffID",
                table: "PaymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentTariff_PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentTariff");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentHistories_PaymentTariffID",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "PaymentTariffID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentTariffID",
                table: "PaymentHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Tariff",
                table: "Payments",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tariff",
                table: "PaymentHistories",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
