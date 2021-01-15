using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _078 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AccountCurrencyRate",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "Money");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AccountCurrencyRate",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "Money",
                oldNullable: true);
        }
    }
}
