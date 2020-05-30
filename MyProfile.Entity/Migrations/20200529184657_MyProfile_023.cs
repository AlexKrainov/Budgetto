using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_ChildAreaID",
                table: "CollectiveAreas");

            migrationBuilder.AlterColumn<int>(
                name: "ChildAreaID",
                table: "CollectiveAreas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AreaID",
                table: "CollectiveAreas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveAreas_AreaID",
                table: "CollectiveAreas",
                column: "AreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_AreaID",
                table: "CollectiveAreas",
                column: "AreaID",
                principalTable: "BudgetAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_ChildAreaID",
                table: "CollectiveAreas",
                column: "ChildAreaID",
                principalTable: "BudgetAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_AreaID",
                table: "CollectiveAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_ChildAreaID",
                table: "CollectiveAreas");

            migrationBuilder.DropIndex(
                name: "IX_CollectiveAreas_AreaID",
                table: "CollectiveAreas");

            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "CollectiveAreas");

            migrationBuilder.AlterColumn<int>(
                name: "ChildAreaID",
                table: "CollectiveAreas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectiveAreas_BudgetAreas_ChildAreaID",
                table: "CollectiveAreas",
                column: "ChildAreaID",
                principalTable: "BudgetAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
