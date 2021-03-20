using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _128 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBankTypes");

            migrationBuilder.AddColumn<string>(
                name: "SiteURL",
                table: "Banks",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeID",
                table: "Banks",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_TypeID",
                table: "Banks",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_AccountTypes_TypeID",
                table: "Banks",
                column: "TypeID",
                principalTable: "AccountTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_AccountTypes_TypeID",
                table: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Banks_TypeID",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "SiteURL",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Banks");

            migrationBuilder.CreateTable(
                name: "AccountBankTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountTypeID = table.Column<int>(nullable: false),
                    BankID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBankTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountBankTypes_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountBankTypes_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBankTypes_AccountTypeID",
                table: "AccountBankTypes",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBankTypes_BankID",
                table: "AccountBankTypes",
                column: "BankID");
        }
    }
}
