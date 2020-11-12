using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_050 : Migration
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
                    Percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InputPromoCode = table.Column<string>(maxLength: 512, nullable: false),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    IsApplied = table.Column<bool>(nullable: false),
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
                name: "IX_PromoCodeHistories_PaymentHistoryID",
                table: "PromoCodeHistories",
                column: "PaymentHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeHistories_PromoCodeID",
                table: "PromoCodeHistories",
                column: "PromoCodeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodeHistories");

            migrationBuilder.DropTable(
                name: "PromoCodes");
        }
    }
}
