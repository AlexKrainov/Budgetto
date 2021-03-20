using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "AccountTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AccountBankTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankTypeID = table.Column<int>(nullable: false),
                    BankID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBankTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountBankTypes_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountBankTypes_AccountTypes_BankTypeID",
                        column: x => x.BankTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBankTypes_BankID",
                table: "AccountBankTypes",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBankTypes_BankTypeID",
                table: "AccountBankTypes",
                column: "BankTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBankTypes");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "AccountTypes");
        }
    }
}
