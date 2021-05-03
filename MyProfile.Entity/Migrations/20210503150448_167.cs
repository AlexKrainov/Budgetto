using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _167 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID2",
                table: "UserLogs",
                newName: "ID3");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PaymentTariffs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "PaymentTariffs");

            migrationBuilder.RenameColumn(
                name: "ID3",
                table: "UserLogs",
                newName: "ID2");
        }
    }
}
