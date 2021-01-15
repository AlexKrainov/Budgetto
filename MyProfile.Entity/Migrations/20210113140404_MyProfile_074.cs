using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_074 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountRecordHistories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionTypeCode = table.Column<string>(nullable: false),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    Cachback = table.Column<decimal>(type: "Money", nullable: false),
                    RacordCurrencyRate = table.Column<decimal>(type: "Money", nullable: false),
                    RecordCurrencyNominal = table.Column<int>(nullable: false),
                    AccountCurrencyRate = table.Column<decimal>(type: "Money", nullable: false),
                    AccountCurrencyNominal = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 264, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    RecordCurrencyID = table.Column<int>(nullable: false),
                    AccountCurrencyID = table.Column<int>(nullable: false),
                    SectionID = table.Column<int>(nullable: false),
                    RecordID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRecordHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRecordHistories_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_AccountID",
                table: "AccountRecordHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecordHistories_RecordID",
                table: "AccountRecordHistories",
                column: "RecordID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRecordHistories");
        }
    }
}
