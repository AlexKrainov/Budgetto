using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _137 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardID",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    SmallLogo = table.Column<string>(maxLength: 256, nullable: true),
                    BigLogo = table.Column<string>(maxLength: 256, nullable: true),
                    IsInterest = table.Column<bool>(nullable: false),
                    Interest = table.Column<decimal>(nullable: false),
                    ServiceCostTo = table.Column<decimal>(nullable: false),
                    ServiceCostFrom = table.Column<decimal>(nullable: false),
                    Cashback = table.Column<decimal>(nullable: false),
                    IsCashback = table.Column<bool>(nullable: false),
                    InterestRate = table.Column<decimal>(nullable: false),
                    CreditLimit = table.Column<decimal>(nullable: false),
                    GracePeriod = table.Column<int>(nullable: false),
                    Raiting = table.Column<int>(nullable: false),
                    bankiruBankName = table.Column<string>(nullable: true),
                    bonuses = table.Column<string>(nullable: true),
                    paymentSystems = table.Column<string>(nullable: true),
                    AccountTypeID = table.Column<int>(nullable: false),
                    BankID = table.Column<int>(nullable: true),
                    PaymentSystemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cards_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_PaymentSystems_PaymentSystemID",
                        column: x => x.PaymentSystemID,
                        principalTable: "PaymentSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardPaymentSystems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardID = table.Column<int>(nullable: false),
                    PaymentSystemID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPaymentSystems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardPaymentSystems_Cards_CardID",
                        column: x => x.CardID,
                        principalTable: "Cards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardPaymentSystems_PaymentSystems_PaymentSystemID",
                        column: x => x.PaymentSystemID,
                        principalTable: "PaymentSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CardID",
                table: "Accounts",
                column: "CardID");

            migrationBuilder.CreateIndex(
                name: "IX_CardPaymentSystems_CardID",
                table: "CardPaymentSystems",
                column: "CardID");

            migrationBuilder.CreateIndex(
                name: "IX_CardPaymentSystems_PaymentSystemID",
                table: "CardPaymentSystems",
                column: "PaymentSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountTypeID",
                table: "Cards",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BankID",
                table: "Cards",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_PaymentSystemID",
                table: "Cards",
                column: "PaymentSystemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Cards_CardID",
                table: "Accounts",
                column: "CardID",
                principalTable: "Cards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Cards_CardID",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "CardPaymentSystems");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CardID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CardID",
                table: "Accounts");
        }
    }
}
