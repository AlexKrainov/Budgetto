using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _079 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountCashback",
                table: "AccountRecordHistories",
                newName: "AccountTotal");

            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalanceCashback",
                table: "AccountRecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalanceCashback",
                table: "AccountRecordHistories");

            migrationBuilder.RenameColumn(
                name: "AccountTotal",
                table: "AccountRecordHistories",
                newName: "AccountCashback");
        }
    }
}
