using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollectiveBudgetID",
                table: "CollectiveBudgetRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_CollectiveBudgetID",
                table: "CollectiveBudgetRequests",
                column: "CollectiveBudgetID");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveBudgetRequests_CollectiveBudgets_CollectiveBudgetID",
                table: "CollectiveBudgetRequests",
                column: "CollectiveBudgetID",
                principalTable: "CollectiveBudgets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveBudgetRequests_CollectiveBudgets_CollectiveBudgetID",
                table: "CollectiveBudgetRequests");

            migrationBuilder.DropIndex(
                name: "IX_CollectiveBudgetRequests_CollectiveBudgetID",
                table: "CollectiveBudgetRequests");

            migrationBuilder.DropColumn(
                name: "CollectiveBudgetID",
                table: "CollectiveBudgetRequests");
        }
    }
}
