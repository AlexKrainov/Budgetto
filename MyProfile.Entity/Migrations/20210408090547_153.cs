using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _153 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreditExpirationDate",
                table: "AccountInfos",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "AccountInfos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GracePeriod",
                table: "AccountInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditExpirationDate",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "GracePeriod",
                table: "AccountInfos");
        }
    }
}
