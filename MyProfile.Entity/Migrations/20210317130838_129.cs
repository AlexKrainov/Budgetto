using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _129 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_AccountTypes_TypeID",
                table: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Banks_TypeID",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "ImageSrc",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "ImageSrc",
                table: "PaymentSystems",
                newName: "Logo");

            migrationBuilder.RenameColumn(
                name: "SiteURL",
                table: "Banks",
                newName: "URL");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AddColumn<int>(
                name: "BankTypeID",
                table: "Banks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Licence",
                table: "Banks",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoCircle",
                table: "Banks",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoRectangle",
                table: "Banks",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Banks",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Raiting",
                table: "Banks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Banks",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tels",
                table: "Banks",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bankiruID",
                table: "Banks",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentSystem",
                table: "AccountTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentAccountID",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BankTypeAccountTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankTypeID = table.Column<int>(nullable: false),
                    AccountTypeID = table.Column<int>(nullable: false)
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
                name: "IX_Banks_BankTypeID",
                table: "Banks",
                column: "BankTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountID",
                table: "Accounts",
                column: "ParentAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BankTypeAccountTypes_AccountTypeID",
                table: "BankTypeAccountTypes",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BankTypeAccountTypes_BankTypeID",
                table: "BankTypeAccountTypes",
                column: "BankTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountID",
                table: "Accounts",
                column: "ParentAccountID",
                principalTable: "Accounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_BankTypes_BankTypeID",
                table: "Banks",
                column: "BankTypeID",
                principalTable: "BankTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Banks_BankTypes_BankTypeID",
                table: "Banks");

            migrationBuilder.DropTable(
                name: "BankTypeAccountTypes");

            migrationBuilder.DropTable(
                name: "BankTypes");

            migrationBuilder.DropIndex(
                name: "IX_Banks_BankTypeID",
                table: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentAccountID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "BankTypeID",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Licence",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "LogoCircle",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "LogoRectangle",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Raiting",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Tels",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "bankiruID",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "IsPaymentSystem",
                table: "AccountTypes");

            migrationBuilder.DropColumn(
                name: "ParentAccountID",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "PaymentSystems",
                newName: "ImageSrc");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Banks",
                newName: "SiteURL");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AddColumn<string>(
                name: "ImageSrc",
                table: "Banks",
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
    }
}
