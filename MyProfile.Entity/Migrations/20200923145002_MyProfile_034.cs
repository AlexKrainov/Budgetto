using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_034 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Option",
                table: "Payments",
                newName: "Tariff");

            migrationBuilder.RenameColumn(
                name: "Option",
                table: "PaymentHistories",
                newName: "Tariff");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tariff",
                table: "Payments",
                newName: "Option");

            migrationBuilder.RenameColumn(
                name: "Tariff",
                table: "PaymentHistories",
                newName: "Option");
        }
    }
}
