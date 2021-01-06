using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_070 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "BudgetRecords",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cashback",
                table: "BudgetRecords",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Icon = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    ImageSrc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    CachBackBalance = table.Column<decimal>(type: "Money", nullable: false),
                    Description = table.Column<string>(maxLength: 264, nullable: true),
                    InterestRate = table.Column<decimal>(nullable: true),
                    CashBackForAllPercent = table.Column<decimal>(nullable: true),
                    IsCachback = table.Column<bool>(nullable: false),
                    IsCachBackMoney = table.Column<bool>(nullable: false),
                    IsOverdraft = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsHide = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    ResetCashBackDate = table.Column<DateTime>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    AccountTypeID = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true),
                    BankID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_AccountID",
                table: "BudgetRecords",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeID",
                table: "Accounts",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BankID",
                table: "Accounts",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyID",
                table: "Accounts",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserID",
                table: "Accounts",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecords_Accounts_AccountID",
                table: "BudgetRecords",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetRecords_Accounts_AccountID",
                table: "BudgetRecords");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_BudgetRecords_AccountID",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Cashback",
                table: "BudgetRecords");
        }
    }
}
