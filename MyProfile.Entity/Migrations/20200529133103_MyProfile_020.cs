using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limits_BudgetAreas_BudgetAreaID",
                table: "Limits");

            migrationBuilder.DropIndex(
                name: "IX_Limits_BudgetAreaID",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "BudgetAreaID",
                table: "Limits");

            migrationBuilder.RenameColumn(
                name: "Money",
                table: "Limits",
                newName: "LimitMoney");

            migrationBuilder.RenameColumn(
                name: "IsConsider",
                table: "BudgetRecords",
                newName: "IsHide");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsing",
                table: "PeriodTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Limits",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "Limits",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                table: "Limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Limits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_DateTimeOfPayment",
                table: "BudgetRecords",
                column: "DateTimeOfPayment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetRecords_DateTimeOfPayment",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "IsUsing",
                table: "PeriodTypes");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Limits");

            migrationBuilder.RenameColumn(
                name: "LimitMoney",
                table: "Limits",
                newName: "Money");

            migrationBuilder.RenameColumn(
                name: "IsHide",
                table: "BudgetRecords",
                newName: "IsConsider");

            migrationBuilder.AddColumn<int>(
                name: "BudgetAreaID",
                table: "Limits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetAreaID",
                table: "Limits",
                column: "BudgetAreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Limits_BudgetAreas_BudgetAreaID",
                table: "Limits",
                column: "BudgetAreaID",
                principalTable: "BudgetAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
