using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OlsonTZID",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_OlsonTZID",
                table: "Users",
                column: "OlsonTZID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_OlsonTZIDs_OlsonTZID",
                table: "Users",
                column: "OlsonTZID",
                principalTable: "OlsonTZIDs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_OlsonTZIDs_OlsonTZID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OlsonTZID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OlsonTZID",
                table: "Users");
        }
    }
}
