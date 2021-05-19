using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "MccCodes",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "t_objectID",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes");

            migrationBuilder.DropColumn(
                name: "t_objectID",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "MccCodes",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MccCodes_Companies_CompanyID",
                table: "MccCodes",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
