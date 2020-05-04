using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limits_BudgetRecords_BudgetRecordID",
                table: "Limits");

            migrationBuilder.DropIndex(
                name: "IX_Limits_BudgetRecordID",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "BudgetRecordID",
                table: "Limits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetRecordID",
                table: "Limits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetRecordID",
                table: "Limits",
                column: "BudgetRecordID");

            migrationBuilder.AddForeignKey(
                name: "FK_Limits_BudgetRecords_BudgetRecordID",
                table: "Limits",
                column: "BudgetRecordID",
                principalTable: "BudgetRecords",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
