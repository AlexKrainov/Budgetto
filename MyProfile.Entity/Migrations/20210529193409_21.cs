using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users");

            migrationBuilder.DropTable(
                name: "PromoCodeHistories");

            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentID",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentID",
                table: "Users",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    LastDatePayment = table.Column<DateTime>(nullable: true),
                    PaymentTariffID = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    LimitCounter = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false),
                    TryCounter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DateClickToPay = table.Column<DateTime>(nullable: true),
                    DateFinisthToPay = table.Column<DateTime>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: false),
                    PaymentID = table.Column<long>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    InputPromoCode = table.Column<string>(maxLength: 512, nullable: false),
                    IsApplied = table.Column<bool>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    PaymentHistoryID = table.Column<Guid>(nullable: false),
                    PromoCodeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PromoCodeHistories_PaymentHistories_PaymentHistoryID",
                        column: x => x.PaymentHistoryID,
                        principalTable: "PaymentHistories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeHistories_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentID",
                table: "Users",
                column: "PaymentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentID",
                table: "PaymentHistories",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeHistories_PaymentHistoryID",
                table: "PromoCodeHistories",
                column: "PaymentHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeHistories_PromoCodeID",
                table: "PromoCodeHistories",
                column: "PromoCodeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
