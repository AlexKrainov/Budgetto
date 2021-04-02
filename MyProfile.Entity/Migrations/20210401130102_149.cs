using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _149 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinishedDeposit",
                table: "AccountInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInterestAccrualDate",
                table: "AccountInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinishedDeposit",
                table: "AccountInfos");

            migrationBuilder.DropColumn(
                name: "LastInterestAccrualDate",
                table: "AccountInfos");
        }
    }
}
