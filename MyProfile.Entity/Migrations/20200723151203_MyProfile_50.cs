using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_50 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "IsShowInCollective",
                table: "Limits");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetYear",
                table: "VisibleElements",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetMonth",
                table: "VisibleElements",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowInCollective",
                table: "VisibleElements",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "IsShowOnDashboards",
                table: "VisibleElements",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "VisibleElementID",
                table: "Limits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Limits_VisibleElementID",
                table: "Limits",
                column: "VisibleElementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Limits_VisibleElements_VisibleElementID",
                table: "Limits",
                column: "VisibleElementID",
                principalTable: "VisibleElements",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Limits_VisibleElements_VisibleElementID",
                table: "Limits");

            migrationBuilder.DropIndex(
                name: "IX_Limits_VisibleElementID",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "IsShowOnDashboards",
                table: "VisibleElements");

            migrationBuilder.DropColumn(
                name: "VisibleElementID",
                table: "Limits");

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetYear",
                table: "VisibleElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShow_BudgetMonth",
                table: "VisibleElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsShowInCollective",
                table: "VisibleElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                table: "Limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowInCollective",
                table: "Limits",
                nullable: false,
                defaultValue: true);
        }
    }
}
