using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    TryCounter = table.Column<int>(nullable: false),
                    LimitCounter = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DateEnterPage = table.Column<DateTime>(nullable: false),
                    DatePayment = table.Column<DateTime>(nullable: true),
                    DateClickToPay = table.Column<DateTime>(nullable: false),
                    DateFinishToPay = table.Column<DateTime>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    PaymentTariffID = table.Column<int>(nullable: true),
                    PromoCodeID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentLogs_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentLogs_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    LastDatePayment = table.Column<DateTime>(nullable: true),
                    PaymentTariffID = table.Column<int>(nullable: false),
                    PromoCodeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InputPromoCode = table.Column<string>(maxLength: 32, nullable: false),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    IsApplied = table.Column<bool>(nullable: false),
                    PromoCodeID = table.Column<int>(nullable: true),
                    PaymentLogID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PromoCodeLogs_PaymentLogs_PaymentLogID",
                        column: x => x.PaymentLogID,
                        principalTable: "PaymentLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeLogs_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    PaymentID = table.Column<Guid>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: true),
                    PromoCodeID = table.Column<int>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_PaymentHistories_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentID",
                table: "PaymentHistories",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PromoCodeID",
                table: "PaymentHistories",
                column: "PromoCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_PaymentTariffID",
                table: "PaymentLogs",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_PromoCodeID",
                table: "PaymentLogs",
                column: "PromoCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_UserID",
                table: "PaymentLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PromoCodeID",
                table: "Payments",
                column: "PromoCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeLogs_PaymentLogID",
                table: "PromoCodeLogs",
                column: "PaymentLogID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeLogs_PromoCodeID",
                table: "PromoCodeLogs",
                column: "PromoCodeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "PromoCodeLogs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentLogs");

            migrationBuilder.DropTable(
                name: "PromoCodes");
        }
    }
}
