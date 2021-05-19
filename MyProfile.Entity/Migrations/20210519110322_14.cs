using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccountOldBalance",
                table: "RecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AccountOldBalanceCashback",
                table: "RecordHistories",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountOldBalance",
                table: "RecordHistories");

            migrationBuilder.DropColumn(
                name: "AccountOldBalanceCashback",
                table: "RecordHistories");
        }
    }
}
