using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTypeAccountTypes");

            migrationBuilder.AddColumn<int>(
                name: "BankTypeID",
                table: "AccountTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_BankTypeID",
                table: "AccountTypes",
                column: "BankTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypes_BankTypes_BankTypeID",
                table: "AccountTypes",
                column: "BankTypeID",
                principalTable: "BankTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypes_BankTypes_BankTypeID",
                table: "AccountTypes");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypes_BankTypeID",
                table: "AccountTypes");

            migrationBuilder.DropColumn(
                name: "BankTypeID",
                table: "AccountTypes");

            migrationBuilder.CreateTable(
                name: "BankTypeAccountTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountTypeID = table.Column<int>(nullable: false),
                    BankTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTypeAccountTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BankTypeAccountTypes_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankTypeAccountTypes_BankTypes_BankTypeID",
                        column: x => x.BankTypeID,
                        principalTable: "BankTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTypeAccountTypes_AccountTypeID",
                table: "BankTypeAccountTypes",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BankTypeAccountTypes_BankTypeID",
                table: "BankTypeAccountTypes",
                column: "BankTypeID");
        }
    }
}
