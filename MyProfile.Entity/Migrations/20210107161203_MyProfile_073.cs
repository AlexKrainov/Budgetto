using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_073 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCachBackMoney",
                table: "Accounts",
                newName: "IsCachbackMoney");

            migrationBuilder.RenameColumn(
                name: "CachBackBalance",
                table: "Accounts",
                newName: "CachbackBalance");

            migrationBuilder.RenameColumn(
                name: "ResetCashBackDate",
                table: "Accounts",
                newName: "ResetCachbackDate");

            migrationBuilder.RenameColumn(
                name: "CashBackForAllPercent",
                table: "Accounts",
                newName: "CachbackForAllPercent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCachbackMoney",
                table: "Accounts",
                newName: "IsCachBackMoney");

            migrationBuilder.RenameColumn(
                name: "CachbackBalance",
                table: "Accounts",
                newName: "CachBackBalance");

            migrationBuilder.RenameColumn(
                name: "ResetCachbackDate",
                table: "Accounts",
                newName: "ResetCashBackDate");

            migrationBuilder.RenameColumn(
                name: "CachbackForAllPercent",
                table: "Accounts",
                newName: "CashBackForAllPercent");
        }
    }
}
