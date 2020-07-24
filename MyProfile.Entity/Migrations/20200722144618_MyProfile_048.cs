using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_048 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowOnDashBoard",
                table: "Goals");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow",
                table: "Limits",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "VisibleElementID",
                table: "Goals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_VisibleElementID",
                table: "Goals",
                column: "VisibleElementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_VisibleElements_VisibleElementID",
                table: "Goals",
                column: "VisibleElementID",
                principalTable: "VisibleElements",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_VisibleElements_VisibleElementID",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_VisibleElementID",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "VisibleElementID",
                table: "Goals");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow",
                table: "Limits",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "IsShowOnDashBoard",
                table: "Goals",
                nullable: false,
                defaultValue: true);
        }
    }
}
