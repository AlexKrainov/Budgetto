using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_075 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastChanges",
                table: "AccountRecordHistories",
                newName: "DateTimeOfPayment");

            migrationBuilder.AlterColumn<decimal>(
                name: "RacordCurrencyRate",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "Money");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTimeOfPayment",
                table: "AccountRecordHistories",
                newName: "LastChanges");

            migrationBuilder.AlterColumn<decimal>(
                name: "RacordCurrencyRate",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "Money",
                oldNullable: true);
        }
    }
}
