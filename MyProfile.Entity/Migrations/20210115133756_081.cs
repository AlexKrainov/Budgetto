using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _081 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountBalanceCashback",
                table: "AccountRecordHistories",
                newName: "AccountNewBalanceCashback");

            migrationBuilder.RenameColumn(
                name: "AccountBalance",
                table: "AccountRecordHistories",
                newName: "AccountNewBalance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountNewBalanceCashback",
                table: "AccountRecordHistories",
                newName: "AccountBalanceCashback");

            migrationBuilder.RenameColumn(
                name: "AccountNewBalance",
                table: "AccountRecordHistories",
                newName: "AccountBalance");
        }
    }
}
