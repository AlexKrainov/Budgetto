using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_032 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentID",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    LastDatePayment = table.Column<DateTime>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: false),
                    Option = table.Column<string>(maxLength: 16, nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DatePayment = table.Column<DateTime>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: false),
                    Option = table.Column<string>(maxLength: 16, nullable: true),
                    DateClickToPay = table.Column<DateTime>(nullable: false),
                    DateFinisthToPay = table.Column<DateTime>(nullable: true),
                    PaymentID = table.Column<int>(nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentID",
                table: "Users",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentID",
                table: "PaymentHistories",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserID",
                table: "Payments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users");

            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentID",
                table: "Users");
        }
    }
}
