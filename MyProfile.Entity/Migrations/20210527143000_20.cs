using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes");

            migrationBuilder.DropIndex(
                name: "IX_MccCodes_CompanyID",
                table: "MccCodes");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "MccCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "MccCodes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MccCodes_CompanyID",
                table: "MccCodes",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
