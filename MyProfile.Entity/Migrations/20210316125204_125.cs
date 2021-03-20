using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _125 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentSystemID",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentSystems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    ImageSrc = table.Column<string>(maxLength: 512, nullable: true),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSystems", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PaymentSystemID",
                table: "Accounts",
                column: "PaymentSystemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_PaymentSystems_PaymentSystemID",
                table: "Accounts",
                column: "PaymentSystemID",
                principalTable: "PaymentSystems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_PaymentSystems_PaymentSystemID",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "PaymentSystems");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_PaymentSystemID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PaymentSystemID",
                table: "Accounts");
        }
    }
}
