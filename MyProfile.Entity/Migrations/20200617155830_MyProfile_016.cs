using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_016 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Charts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisibleElementID",
                table: "Charts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Charts_VisibleElementID",
                table: "Charts",
                column: "VisibleElementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_VisibleElements_VisibleElementID",
                table: "Charts",
                column: "VisibleElementID",
                principalTable: "VisibleElements",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_VisibleElements_VisibleElementID",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_VisibleElementID",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "VisibleElementID",
                table: "Charts");
        }
    }
}
