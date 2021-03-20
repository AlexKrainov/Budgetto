using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _133 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AccountHistories");

            migrationBuilder.AddColumn<string>(
                name: "BrandColor",
                table: "Banks",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CachbackBalance",
                table: "AccountHistories",
                type: "Money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyValue",
                table: "AccountHistories",
                type: "Money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewBalance",
                table: "AccountHistories",
                type: "Money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldBalance",
                table: "AccountHistories",
                type: "Money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueFrom",
                table: "AccountHistories",
                type: "Money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueTo",
                table: "AccountHistories",
                type: "Money",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandColor",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "CachbackBalance",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "CurrencyValue",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "NewBalance",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "OldBalance",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "ValueFrom",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "ValueTo",
                table: "AccountHistories");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true);
        }
    }
}
