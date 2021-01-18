using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _085 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRecordHistories");

            migrationBuilder.CreateTable(
                name: "AccountHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionType = table.Column<string>(maxLength: 16, nullable: false),
                    OldAccountStateJson = table.Column<string>(nullable: true),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    AccountID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionTypeCode = table.Column<string>(nullable: false),
                    RecordTotal = table.Column<decimal>(type: "Money", nullable: false),
                    RecordCachback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountNewBalance = table.Column<decimal>(type: "Money", nullable: false),
                    AccountNewBalanceCashback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountTotal = table.Column<decimal>(type: "Money", nullable: false),
                    AccountCashback = table.Column<decimal>(type: "Money", nullable: false),
                    RacordCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    RecordCurrencyNominal = table.Column<int>(nullable: false),
                    AccountCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    AccountCurrencyNominal = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 264, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    RecordID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: true),
                    RecordCurrencyID = table.Column<int>(nullable: true),
                    AccountCurrencyID = table.Column<int>(nullable: true),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Currencies_AccountCurrencyID",
                        column: x => x.AccountCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Currencies_RecordCurrencyID",
                        column: x => x.RecordCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordHistories_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistories_AccountID",
                table: "AccountHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_AccountCurrencyID",
                table: "RecordHistories",
                column: "AccountCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_AccountID",
                table: "RecordHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_RecordCurrencyID",
                table: "RecordHistories",
                column: "RecordCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_RecordID",
                table: "RecordHistories",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_SectionID",
                table: "RecordHistories",
                column: "SectionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountHistories");

            migrationBuilder.DropTable(
                name: "RecordHistories");

            migrationBuilder.CreateTable(
                name: "AccountRecordHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountCashback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountCurrencyID = table.Column<int>(nullable: true),
                    AccountCurrencyNominal = table.Column<int>(nullable: false),
                    AccountCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    AccountID = table.Column<int>(nullable: true),
                    AccountNewBalance = table.Column<decimal>(type: "Money", nullable: false),
                    AccountNewBalanceCashback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountTotal = table.Column<decimal>(type: "Money", nullable: false),
                    ActionTypeCode = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(maxLength: 264, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    RacordCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    RecordCachback = table.Column<decimal>(type: "Money", nullable: false),
                    RecordCurrencyID = table.Column<int>(nullable: true),
                    RecordCurrencyNominal = table.Column<int>(nullable: false),
                    RecordID = table.Column<int>(nullable: false),
                    RecordTotal = table.Column<decimal>(type: "Money", nullable: false),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRecordHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_Currencies_AccountCurrencyID",
                        column: x => x.AccountCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_Currencies_RecordCurrencyID",
                        column: x => x.RecordCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_AccountCurrencyID",
                table: "AccountRecordHistories",
                column: "AccountCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_AccountID",
                table: "AccountRecordHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_RecordCurrencyID",
                table: "AccountRecordHistories",
                column: "RecordCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_RecordID",
                table: "AccountRecordHistories",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_SectionID",
                table: "AccountRecordHistories",
                column: "SectionID");
        }
    }
}
