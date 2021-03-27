using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _138 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "Cards");

            migrationBuilder.AddColumn<int>(
                name: "bankiruCardID",
                table: "Cards",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bankiruCardID",
                table: "Cards");

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "Cards",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
