using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_076 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "AccountRecordHistories",
                newName: "OldRecordTotal");

            migrationBuilder.RenameColumn(
                name: "Cachback",
                table: "AccountRecordHistories",
                newName: "OldRecordCachback");

            migrationBuilder.AddColumn<decimal>(
                name: "NewAccountBalance",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewAccountCashback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewRecordCachback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewRecordTotal",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OldAccountBalance",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OldAccountCashback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewAccountBalance",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewAccountCashback",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewRecordCachback",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "NewRecordTotal",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "OldAccountBalance",
                table: "AccountRecordHistories");

            migrationBuilder.DropColumn(
                name: "OldAccountCashback",
                table: "AccountRecordHistories");

            migrationBuilder.RenameColumn(
                name: "OldRecordTotal",
                table: "AccountRecordHistories",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "OldRecordCachback",
                table: "AccountRecordHistories",
                newName: "Cachback");
        }
    }
}
