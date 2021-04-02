using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _148 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "CapitalizationOfDeposit",
                table: "AccountInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestBalance",
                table: "AccountInfos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InterestNextDate",
                table: "AccountInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapitalizationOfDeposit",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "InterestBalance",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "InterestNextDate",
                table: "AccountInfos");

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "Accounts",
                nullable: true);
        }
    }
}
