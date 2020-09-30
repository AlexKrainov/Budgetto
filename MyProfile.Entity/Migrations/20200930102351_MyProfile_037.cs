using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_037 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetSections_Users_UserID",
                table: "BudgetSections");

            migrationBuilder.DropIndex(
                name: "IX_BudgetSections_UserID",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "BudgetSections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_UserID",
                table: "BudgetSections",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetSections_Users_UserID",
                table: "BudgetSections",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
